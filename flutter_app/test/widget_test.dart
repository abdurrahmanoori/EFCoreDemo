import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:pharmacy_manager/api/api_client.dart';
import 'package:pharmacy_manager/main.dart';

void main() {
  testWidgets('login screen renders controls', (tester) async {
    final api = ApiClient(
      client: MockClient((_) async => http.Response('{}', 200)),
      baseUrl: 'http://test/api',
    );
    await tester.pumpWidget(MaterialApp(home: LoginPage(api: api)));
    expect(find.text('Pharmacy Manager'), findsOneWidget);
    expect(find.text('Username'), findsOneWidget);
    expect(find.text('Password'), findsOneWidget);
    expect(find.text('Sign in'), findsOneWidget);
  });
}