using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi;
using TDMK_API_DEMO.Data;
using TDMK_API_DEMO.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSignalR();

builder.Services.AddControllers();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddApplicationInsightsTelemetry();

// Cấu hình Swagger chi tiết
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "TDMK IoT API - Hoang Thai Thuy",
        Description = "API hệ thống tracking ảnh và số liệu Realtime",
    });
    
});

//use memeory cache
builder.Services.AddMemoryCache();

/// Allow cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true) // Cho phép TẤT CẢ các nguồn (Origins)
              .AllowAnyHeader()              // Cho phép TẤT CẢ các loại Header
              .AllowAnyMethod()              // Cho phép TẤT CẢ các phương thức (GET, POST,...)
              .AllowCredentials();           // BẮT BUỘC phải có để SignalR hoạt động
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        // Tự động thử lại nếu DB chưa khởi động kịp (rất quan trọng trong Docker)
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));


var app = builder.Build();

// Auto generate table in databse
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        // Lệnh này thay thế việc gõ "dotnet ef database update" thủ công
        context.Database.Migrate();
        Console.WriteLine("Database đã được khởi tạo và cập nhật bảng thành công!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Lỗi khi khởi tạo Database: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Tạo ra file swagger.json
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = "swagger"; // Truy cập bằng đường dẫn: localhost:5000/swagger
    });
}
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();

var uploadFolder = app.Configuration.GetValue<string>("FileStorage:UploadFolder")
                   ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.GetFullPath(uploadFolder)),
    RequestPath = "/uploads"
});

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.MapHub<MonitorHub>("/hubs/monitor");

app.Run();
