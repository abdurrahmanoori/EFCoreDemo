import 'package:flutter/material.dart';
import 'api/api_client.dart';

void main() => runApp(PharmacyApp(api: ApiClient()));

class PharmacyApp extends StatelessWidget {
  const PharmacyApp({super.key, required this.api});
  final ApiClient api;

  @override
  Widget build(BuildContext context) => MaterialApp(
    title: 'Pharmacy Manager',
    debugShowCheckedModeBanner: false,
    theme: ThemeData(
      colorSchemeSeed: const Color(0xFF167D6A),
      useMaterial3: true,
      scaffoldBackgroundColor: const Color(0xFFF6F8FA),
    ),
    home: LoginPage(api: api),
  );
}

class LoginPage extends StatefulWidget {
  const LoginPage({super.key, required this.api});
  final ApiClient api;
  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final username = TextEditingController();
  final password = TextEditingController();
  bool loading = false;
  String? error;

  Future<void> submit() async {
    setState(() { loading = true; error = null; });
    try {
      await widget.api.login(username.text.trim(), password.text);
      if (!mounted) return;
      Navigator.of(context).pushReplacement(
        MaterialPageRoute(builder: (_) => PharmacyShell(api: widget.api)),
      );
    } catch (_) {
      if (mounted) setState(() => error = 'Login failed. Check credentials and API connectivity.');
    } finally {
      if (mounted) setState(() => loading = false);
    }
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    body: Center(
      child: SizedBox(
        width: 420,
        child: Card(
          child: Padding(
            padding: const EdgeInsets.all(32),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                const Icon(Icons.local_pharmacy_outlined, size: 56),
                const SizedBox(height: 16),
                Text('Pharmacy Manager',
                  textAlign: TextAlign.center,
                  style: Theme.of(context).textTheme.headlineMedium),
                const SizedBox(height: 28),
                TextField(controller: username,
                  decoration: const InputDecoration(labelText: 'Username', border: OutlineInputBorder())),
                const SizedBox(height: 16),
                TextField(controller: password, obscureText: true,
                  decoration: const InputDecoration(labelText: 'Password', border: OutlineInputBorder())),
                if (error != null)
                  Padding(
                    padding: const EdgeInsets.only(top: 12),
                    child: Text(error!, style: TextStyle(color: Theme.of(context).colorScheme.error)),
                  ),
                const SizedBox(height: 20),
                FilledButton(
                  onPressed: loading ? null : submit,
                  child: Text(loading ? 'Signing in…' : 'Sign in'),
                ),
              ],
            ),
          ),
        ),
      ),
    ),
  );
}

class PharmacyShell extends StatefulWidget {
  const PharmacyShell({super.key, required this.api});
  final ApiClient api;
  @override
  State<PharmacyShell> createState() => _PharmacyShellState();
}

class _PharmacyShellState extends State<PharmacyShell> {
  int index = 0;

  static const destinations = [
    NavigationRailDestination(icon: Icon(Icons.dashboard_outlined), selectedIcon: Icon(Icons.dashboard), label: Text('Dashboard')),
    NavigationRailDestination(icon: Icon(Icons.medication_outlined), selectedIcon: Icon(Icons.medication), label: Text('Medicines')),
    NavigationRailDestination(icon: Icon(Icons.inventory_2_outlined), selectedIcon: Icon(Icons.inventory_2), label: Text('Stock')),
    NavigationRailDestination(icon: Icon(Icons.description_outlined), selectedIcon: Icon(Icons.description), label: Text('Prescriptions')),
    NavigationRailDestination(icon: Icon(Icons.point_of_sale_outlined), selectedIcon: Icon(Icons.point_of_sale), label: Text('Sales')),
  ];

  Widget page() => [
    DashboardPage(api: widget.api),
    MedicinePage(api: widget.api),
    StockPage(api: widget.api),
    SimpleListPage(title: 'Prescriptions', future: widget.api.prescriptions(), titleKey: 'prescriptionNumber', subtitleKey: 'diagnosis'),
    SimpleListPage(title: 'Sales', future: widget.api.sales(), titleKey: 'invoiceNumber', subtitleKey: 'total'),
  ][index];

  @override
  Widget build(BuildContext context) {
    final wide = MediaQuery.sizeOf(context).width >= 850;
    if (!wide) {
      return Scaffold(
        appBar: AppBar(title: const Text('Pharmacy Manager')),
        drawer: NavigationDrawer(
          selectedIndex: index,
          onDestinationSelected: (value) {
            setState(() => index = value);
            Navigator.pop(context);
          },
          children: [
            const Padding(padding: EdgeInsets.all(20), child: Text('Operations')),
            ...destinations.map((d) => NavigationDrawerDestination(
              icon: d.icon, selectedIcon: d.selectedIcon, label: d.label)),
          ],
        ),
        body: page(),
      );
    }

    return Scaffold(
      body: Row(
        children: [
          NavigationRail(
            extended: true,
            selectedIndex: index,
            onDestinationSelected: (value) => setState(() => index = value),
            leading: const Padding(
              padding: EdgeInsets.symmetric(vertical: 24),
              child: Icon(Icons.local_pharmacy, size: 36),
            ),
            destinations: destinations,
          ),
          const VerticalDivider(width: 1),
          Expanded(child: page()),
        ],
      ),
    );
  }
}

class PageFrame extends StatelessWidget {
  const PageFrame({super.key, required this.title, required this.child});
  final String title;
  final Widget child;

  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.all(24),
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(title, style: Theme.of(context).textTheme.headlineMedium),
        const SizedBox(height: 20),
        Expanded(child: child),
      ],
    ),
  );
}

class DashboardPage extends StatelessWidget {
  const DashboardPage({super.key, required this.api});
  final ApiClient api;

  @override
  Widget build(BuildContext context) => PageFrame(
    title: 'Dashboard',
    child: FutureBuilder<Map<String, dynamic>>(
      future: api.dashboard(),
      builder: (context, snapshot) {
        if (snapshot.hasError) return Center(child: Text(snapshot.error.toString()));
        if (!snapshot.hasData) return const Center(child: CircularProgressIndicator());
        final d = snapshot.data!;
        return GridView.count(
          crossAxisCount: MediaQuery.sizeOf(context).width > 1100 ? 4 : 2,
          mainAxisSpacing: 16,
          crossAxisSpacing: 16,
          childAspectRatio: 2.2,
          children: [
            StatCard('Medicines', (d['medicineCount'] ?? 0).toString(), Icons.medication),
            StatCard('Low stock', (d['lowStock'] ?? 0).toString(), Icons.warning_amber),
            StatCard('Expiring ≤ 90 days', (d['expiringBatches'] ?? 0).toString(), Icons.schedule),
            StatCard('Expired batches', (d['expiredBatches'] ?? 0).toString(), Icons.event_busy),
          ],
        );
      },
    ),
  );
}

class StatCard extends StatelessWidget {
  const StatCard(this.label, this.value, this.icon, {super.key});
  final String label;
  final String value;
  final IconData icon;

  @override
  Widget build(BuildContext context) => Card(
    child: Padding(
      padding: const EdgeInsets.all(20),
      child: Row(
        children: [
          CircleAvatar(child: Icon(icon)),
          const SizedBox(width: 16),
          Column(
            mainAxisAlignment: MainAxisAlignment.center,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(value, style: Theme.of(context).textTheme.headlineMedium),
              Text(label),
            ],
          ),
        ],
      ),
    ),
  );
}

class MedicinePage extends StatefulWidget {
  const MedicinePage({super.key, required this.api});
  final ApiClient api;
  @override
  State<MedicinePage> createState() => _MedicinePageState();
}

class _MedicinePageState extends State<MedicinePage> {
  String search = '';

  @override
  Widget build(BuildContext context) => PageFrame(
    title: 'Medicines',
    child: Column(
      children: [
        TextField(
          onChanged: (value) => setState(() => search = value),
          decoration: const InputDecoration(
            prefixIcon: Icon(Icons.search),
            hintText: 'Search code, brand, generic name or barcode',
            border: OutlineInputBorder(),
          ),
        ),
        const SizedBox(height: 12),
        Expanded(
          child: FutureBuilder<List<dynamic>>(
            future: widget.api.medicines(search),
            builder: (context, snapshot) {
              if (!snapshot.hasData) return const Center(child: CircularProgressIndicator());
              return ListView.separated(
                itemCount: snapshot.data!.length,
                separatorBuilder: (_, __) => const Divider(height: 1),
                itemBuilder: (_, i) {
                  final m = snapshot.data![i] as Map<String, dynamic>;
                  return ListTile(
                    title: Text("${m['brandName'] ?? ''} ${m['strength'] ?? ''}"),
                    subtitle: Text("${m['genericName'] ?? ''} • ${m['category'] ?? ''}"),
                    trailing: Text((m['sellingPrice'] ?? '').toString()),
                  );
                },
              );
            },
          ),
        ),
      ],
    ),
  );
}

class StockPage extends StatelessWidget {
  const StockPage({super.key, required this.api});
  final ApiClient api;

  @override
  Widget build(BuildContext context) => PageFrame(
    title: 'Stock & FEFO',
    child: FutureBuilder<List<dynamic>>(
      future: api.stock(),
      builder: (context, snapshot) {
        if (!snapshot.hasData) return const Center(child: CircularProgressIndicator());
        return ListView.builder(
          itemCount: snapshot.data!.length,
          itemBuilder: (_, i) {
            final x = snapshot.data![i] as Map<String, dynamic>;
            final available = x['available'] as int? ?? 0;
            final reorder = x['reorderLevel'] as int? ?? 0;
            return Card(
              child: ListTile(
                leading: Icon(available <= reorder ? Icons.warning_amber : Icons.inventory_2_outlined),
                title: Text((x['brandName'] ?? '').toString()),
                subtitle: Text('Available: $available • Reorder: $reorder'),
                trailing: Text((x['nextExpiry'] ?? 'No batch').toString()),
              ),
            );
          },
        );
      },
    ),
  );
}

class SimpleListPage extends StatelessWidget {
  const SimpleListPage({
    super.key,
    required this.title,
    required this.future,
    required this.titleKey,
    required this.subtitleKey,
  });

  final String title;
  final Future<List<dynamic>> future;
  final String titleKey;
  final String subtitleKey;

  @override
  Widget build(BuildContext context) => PageFrame(
    title: title,
    child: FutureBuilder<List<dynamic>>(
      future: future,
      builder: (context, snapshot) {
        if (!snapshot.hasData) return const Center(child: CircularProgressIndicator());
        return ListView.separated(
          itemCount: snapshot.data!.length,
          separatorBuilder: (_, __) => const Divider(height: 1),
          itemBuilder: (_, i) {
            final x = snapshot.data![i] as Map<String, dynamic>;
            return ListTile(
              title: Text((x[titleKey] ?? '').toString()),
              subtitle: Text((x[subtitleKey] ?? '').toString()),
            );
          },
        );
      },
    ),
  );
}