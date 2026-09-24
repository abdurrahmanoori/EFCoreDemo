import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:pharmacy_manager/api/api_client.dart';

void main() {
  test('login stores bearer token', () async {
    final client = MockClient((request) async {
      expect(request.url.path, '/api/auth/login');
      return http.Response('{"token":"abc","fullName":"Admin","role":0}', 200);
    });
    final api = ApiClient(client: client, baseUrl: 'http://test/api');
    await api.login('admin', 'secret');
    expect(api.token, 'abc');
  });

  test('authenticated request sends bearer token', () async {
    final client = MockClient((request) async {
      if (request.url.path == '/api/auth/login') {
        return http.Response('{"token":"xyz"}', 200);
      }
      expect(request.headers['Authorization'], 'Bearer xyz');
      return http.Response('{"medicineCount":1}', 200);
    });
    final api = ApiClient(client: client, baseUrl: 'http://test/api');
    await api.login('a', 'b');
    final result = await api.dashboard();
    expect(result['medicineCount'], 1);
  });

  test('failed response throws ApiException', () async {
    final api = ApiClient(
      client: MockClient((_) async => http.Response('failure', 500)),
      baseUrl: 'http://test/api',
    );
    expect(api.dashboard(), throwsA(isA<ApiException>()));
  });
}