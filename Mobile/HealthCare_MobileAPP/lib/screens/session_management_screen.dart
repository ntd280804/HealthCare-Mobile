import 'package:flutter/material.dart';

import '../services/api_service.dart';

class SessionManagementScreen extends StatefulWidget {
  const SessionManagementScreen({super.key});

  @override
  State<SessionManagementScreen> createState() => _SessionManagementScreenState();
}

class _SessionManagementScreenState extends State<SessionManagementScreen> {
  final ApiService _api = ApiService();
  bool _loading = true;
  List<SessionInfo> _sessions = [];
  String? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final sessions = await _api.getMySessions();
      setState(() {
        _sessions = sessions;
        _loading = false;
      });
    } catch (e) {
      setState(() {
        _error = 'Không tải được danh sách phiên: $e';
        _loading = false;
      });
    }
  }

  Future<void> _logoutAll({required bool includeCurrent}) async {
    final ok = await _api.logoutAllSessions(includeCurrent: includeCurrent);
    if (!mounted) return;

    // Nếu yêu cầu "đăng xuất tất cả" bao gồm cả phiên hiện tại trên điện thoại,
    // thì sau khi server xử lý xong ta sẽ đưa người dùng về màn hình đăng nhập.
    if (ok && includeCurrent) {
      try {
        // Gọi thêm logout để dọn dẹp token/session local; lỗi sẽ được bỏ qua.
        await _api.logout();
      } catch (_) {}

      Navigator.of(context).pushNamedAndRemoveUntil('/login', (route) => false);
      return;
    }

    final snackBar = SnackBar(
      content: Text(ok
          ? (includeCurrent ? 'Đã yêu cầu đăng xuất tất cả phiên.' : 'Đã yêu cầu đăng xuất các phiên khác.')
          : 'Không thể gửi yêu cầu đăng xuất.'),
    );
    ScaffoldMessenger.of(context).showSnackBar(snackBar);
    await _load();
  }

  Future<void> _logoutPlatform(String platform) async {
    final ok = await _api.logoutPlatform(platform, includeCurrent: false);
    if (!mounted) return;
    final snackBar = SnackBar(
      content: Text(ok
          ? 'Đã yêu cầu đăng xuất tất cả phiên trên $platform.'
          : 'Không thể gửi yêu cầu đăng xuất platform.'),
    );
    ScaffoldMessenger.of(context).showSnackBar(snackBar);
    await _load();
  }

  @override
  Widget build(BuildContext context) {
    final currentPlatform = 'MOBILE';
    final platforms = _sessions.map((s) => s.platform).toSet().toList();
    final otherPlatforms = platforms.where((p) => p != currentPlatform).toList();

    return Scaffold(
      appBar: AppBar(
        title: const Text('Quản lý phiên đăng nhập'),
      ),
      body: RefreshIndicator(
        onRefresh: _load,
        child: _loading
            ? const Center(child: CircularProgressIndicator())
            : _error != null
                ? ListView(
                    children: [
                      Padding(
                        padding: const EdgeInsets.all(16),
                        child: Text(_error!, style: const TextStyle(color: Colors.red)),
                      ),
                    ],
                  )
                : ListView(
                    padding: const EdgeInsets.all(16),
                    children: [
                      Row(
                        children: [
                          Expanded(
                            child: ElevatedButton.icon(
                              onPressed: () async {
                                final confirm = await showDialog<bool>(
                                      context: context,
                                      builder: (ctx) => AlertDialog(
                                        title: const Text('Đăng xuất các phiên khác'),
                                        content: const Text(
                                            'Đăng xuất tất cả phiên khác, giữ lại phiên hiện tại trên điện thoại này?'),
                                        actions: [
                                          TextButton(
                                            onPressed: () => Navigator.of(ctx).pop(false),
                                            child: const Text('HỦY'),
                                          ),
                                          TextButton(
                                            onPressed: () => Navigator.of(ctx).pop(true),
                                            child: const Text('ĐỒNG Ý'),
                                          ),
                                        ],
                                      ),
                                    ) ??
                                    false;
                                if (confirm) {
                                  await _logoutAll(includeCurrent: false);
                                }
                              },
                              icon: const Icon(Icons.logout),
                              label: const Text('Đăng xuất các phiên khác'),
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 8),
                      Row(
                        children: [
                          Expanded(
                            child: OutlinedButton.icon(
                              onPressed: () async {
                                final confirm = await showDialog<bool>(
                                      context: context,
                                      builder: (ctx) => AlertDialog(
                                        title: const Text('Đăng xuất tất cả'),
                                        content: const Text(
                                            'Đăng xuất TẤT CẢ phiên (bao gồm cả phiên hiện tại trên điện thoại)?'),
                                        actions: [
                                          TextButton(
                                            onPressed: () => Navigator.of(ctx).pop(false),
                                            child: const Text('HỦY'),
                                          ),
                                          TextButton(
                                            onPressed: () => Navigator.of(ctx).pop(true),
                                            child: const Text('ĐỒNG Ý'),
                                          ),
                                        ],
                                      ),
                                    ) ??
                                    false;
                                if (confirm) {
                                  await _logoutAll(includeCurrent: true);
                                }
                              },
                              icon: const Icon(Icons.close),
                              label: const Text('Đăng xuất tất cả'),
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 16),
                      if (otherPlatforms.isNotEmpty)
                        Card(
                          child: Padding(
                            padding: const EdgeInsets.all(16),
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                const Text(
                                  'Đăng xuất thiết bị khác',
                                  style: TextStyle(fontSize: 16, fontWeight: FontWeight.w600),
                                ),
                                const SizedBox(height: 8),
                                Wrap(
                                  spacing: 8,
                                  children: otherPlatforms
                                      .map(
                                        (p) => ActionChip(
                                          label: Text('Đăng xuất $p'),
                                          onPressed: () => _logoutPlatform(p),
                                        ),
                                      )
                                      .toList(),
                                ),
                              ],
                            ),
                          ),
                        ),
                      const SizedBox(height: 16),
                      Text(
                        'Danh sách phiên đang mở (${_sessions.length})',
                        style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                      ),
                      const SizedBox(height: 8),
                      if (_sessions.isEmpty)
                        const Card(
                          child: Padding(
                            padding: EdgeInsets.all(16),
                            child: Text('Không có phiên nào.'),
                          ),
                        )
                      else
                        ..._sessions.map(
                          (s) => Card(
                            child: ListTile(
                              title: Text('${s.platform} - ${s.sessionId}'),
                              subtitle: Text('SID: ${s.oracleSid} • Trạng thái: ${s.state}'),
                              trailing: Text(
                                s.platform == currentPlatform ? 'Hiện tại?' : '',
                                style: const TextStyle(color: Colors.green),
                              ),
                            ),
                          ),
                        ),
                    ],
                  ),
      ),
    );
  }
}


