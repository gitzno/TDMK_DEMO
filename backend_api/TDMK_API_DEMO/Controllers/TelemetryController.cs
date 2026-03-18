using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TDMK_API_DEMO.Data;
using TDMK_API_DEMO.DTOs;
using TDMK_API_DEMO.Hubs;
using TDMK_API_DEMO.Models;

namespace TDMK_API_DEMO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelemetryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<MonitorHub> _hubContext; // SignalR
        private readonly string _uploadPath;
        private readonly IMemoryCache _cache;
        private const string ReportsCacheKey = "LatestReports_Key";

        public TelemetryController(AppDbContext context, IHubContext<MonitorHub> hubContext, IConfiguration configuration, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
            _hubContext = hubContext;

            // Lấy giá trị từ FileStorage:UploadFolder (Dù là từ JSON hay .env)
            var configFolder = configuration["FileStorage:UploadFolder"] ?? "wwwroot/uploads";

            // Nếu đường dẫn là tương đối, ta kết hợp với CurrentDirectory
            _uploadPath = Path.IsPathRooted(configFolder)
                ? configFolder
                : Path.Combine(Directory.GetCurrentDirectory(), configFolder);

            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);
        }

        [HttpPost]
        [Consumes("multipart/form-data")] // Ép Swagger hiểu đây là form upload
        public async Task<IActionResult> Upload([FromForm] DTOTelemetry dto)
        {
            string? imageUrl = null;

            // 1. Xử lý lưu File ảnh
            if (dto.Image != null)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                var filePath = Path.Combine(_uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }
                imageUrl = $"/uploads/{fileName}";
            }

            // 2. Lưu vào Database
            var record = new Telemetry { Value = dto.Value, ImageUrl = imageUrl };
            _context.Telemetries.Add(record);
            await _context.SaveChangesAsync();

            // 3. Đẩy Realtime qua SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveData", new
            {
                value = dto.Value,
                imageUrl = record.ImageUrl, // Đường dẫn ảnh sau khi lưu
                timestamp = DateTime.Now.ToString("HH:mm:ss")
            });
            _cache.Remove($"{ReportsCacheKey}_P1_S10"); // Xóa trang đầu tiên vì nó hay thay đổi nhất

            return Ok(record);
        }

        [HttpGet("reports")]
        public async Task<IActionResult> GetReports([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // Tạo Key riêng biệt cho từng trang để tránh lẫn lộn dữ liệu
            string cacheKey = $"{ReportsCacheKey}_P{page}_S{pageSize}";

            // 1. Kiểm tra xem Cache đã có dữ liệu chưa
            if (!_cache.TryGetValue(cacheKey, out object reportsData))
            {
                // 2. Nếu CHƯA CÓ, thực hiện truy vấn DB
                var totalRecords = await _context.Telemetries.CountAsync();
                var data = await _context.Telemetries
                    .AsNoTracking()
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                reportsData = new { Total = totalRecords, Data = data };

                // 3. Thiết lập cấu hình Cache
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2)) // Hết hạn nếu không ai gọi trong 2 phút
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)) // Chắc chắn hết hạn sau 10 phút
                    .SetPriority(CacheItemPriority.Normal);

                // 4. Lưu vào Cache
                _cache.Set(cacheKey, reportsData, cacheOptions);

                Console.WriteLine("==> Lấy dữ liệu từ DATABASE");
            }
            else
            {
                Console.WriteLine("==> Lấy dữ liệu từ CACHE (Siêu nhanh)");
            }

            return Ok(reportsData);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            string cacheKey = $"Telemetry_Detail_{id}";

            // 1. Thử lấy từ Cache
            if (!_cache.TryGetValue(cacheKey, out object detail))
            {
                // 2. Nếu không có, truy vấn DB
                var record = await _context.Telemetries
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (record == null) return NotFound(new { message = "Không tìm thấy dữ liệu" });

                // 3. Xử lý URL ảnh để Frontend hiển thị được
                // Giả sử ImageUrl trong DB là "abc.jpg", ta cần trả về URL đầy đủ
                var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

                detail = new
                {
                    record.Id,
                    record.Value,
                    // Nếu lưu trong wwwroot/uploads:
                    FullImageUrl = $"{baseUrl}/uploads/{record.ImageUrl}",
                    CreatedAt = record.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")
                };

                // 4. Lưu vào Cache trong 5 phút
                _cache.Set(cacheKey, detail, TimeSpan.FromMinutes(5));
            }

            return Ok(detail);
        }   
    }
} 
