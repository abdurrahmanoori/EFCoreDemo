import 'dart:convert';
import 'package:http/http.dart' as http;

class ApiClient {
  ApiClient({http.Client? client, String? baseUrl})
      : _client = client ?? http.Client(),
        baseUrl = baseUrl ?? const String.fromEnvironment(
          'API_BASE_URL',
          defaultValue: 'http://localhost:5000/api',
        );

  final http.Client _client;
  final String baseUrl;
  String? token;

  Map<String, String> get headers => {
    'Content-Type': 'application/json',
    if (token != null) 'Authorization': 'Bearer ' + token!,
  };

  Future<Map<String, dynamic>> login(String username, String password) async {
    final response = await _client.post(
      Uri.parse(baseUrl + '/auth/login'),
      headers: headers,
      body: jsonEncode({'username': username, 'password': password}),
    );
    _ensureSuccess(response);
    final data = jsonDecode(response.body) as Map<String, dynamic>;
    token = data['token'] as String;
    return data;
  }

  Future<Map<String, dynamic>> dashboard() => _getMap('/dashboard');
  Future<List<dynamic>> medicines([String search = '']) =>
      _getList('/catalog/medicines?search=' + Uri.encodeQueryComponent(search));
  Future<List<dynamic>> stock() => _getList('/inventory/stock');
  Future<List<dynamic>> expiring([int days = 90]) =>
      _getList('/inventory/expiring?days=' + days.toString());
  Future<List<dynamic>> prescriptions() => _getList('/prescriptions');
  Future<List<dynamic>> sales() => _getList('/sales');

  Future<Map<String, dynamic>> _getMap(String path) async {
    final response = await _client.get(Uri.parse(baseUrl + path), headers: headers);
    _ensureSuccess(response);
    return jsonDecode(response.body) as Map<String, dynamic>;
  }

  Future<List<dynamic>> _getList(String path) async {
    final response = await _client.get(Uri.parse(baseUrl + path), headers: headers);
    _ensureSuccess(response);
    return jsonDecode(response.body) as List<dynamic>;
  }

  void _ensureSuccess(http.Response response) {
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException(response.statusCode, response.body);
    }
  }
}

class ApiException implements Exception {
  ApiException(this.statusCode, this.body);
  final int statusCode;
  final String body;
  @override
  String toString() => 'API ' + statusCode.toString() + ': ' + body;
}