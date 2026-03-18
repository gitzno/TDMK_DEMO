using Microsoft.AspNetCore.SignalR;

namespace TDMK_API_DEMO.Hubs
{
    public class MonitorHub : Hub
    {
        // 1. Khi một Client kết nối thành công
        public override async Task OnConnectedAsync()
        {

            string connectionId = Context.ConnectionId;
            Console.WriteLine($"==> [SignalR] Thiết bị mới kết nối: {connectionId}");

            // Gửi lời chào ngược lại cho Client vừa kết nối
            await Clients.Caller.SendAsync("Welcome", $"Chào bạn! ID kết nối của bạn là: {connectionId}");

            await base.OnConnectedAsync();
        }

        // 2. Khi Client ngắt kết nối
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"==> [SignalR] Thiết bị ngắt kết nối: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        // 3. Hàm để Client "Đăng ký" theo dõi một Device cụ thể (Nếu sau này Thụy có nhiều máy)
        public async Task JoinDeviceGroup(string deviceId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, deviceId);
            Console.WriteLine($"==> [SignalR] {Context.ConnectionId} đã tham gia nhóm theo dõi: {deviceId}");
        }

        // 4. Hàm để Client gửi tin nhắn phản hồi hoặc yêu cầu Snapshot mới
        public async Task RequestUpdate(string message)
        {
            Console.WriteLine($"==> [SignalR] Client yêu cầu: {message}");
            // Phản hồi lại cho Client
            await Clients.Caller.SendAsync("Notify", "Yêu cầu của bạn đã được tiếp nhận.");
        }
    }
}