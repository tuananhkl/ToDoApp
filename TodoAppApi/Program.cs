using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using TodoAppApi.Context;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSystemd();

// Add services to the container.

    builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    // builder.Services.AddDbContext<AppDbContext>(options =>
    //     options.UseInMemoryDatabase("TodoDb"));

    builder.Services.AddDbContext<AppDbContext>(options => 
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    
    var app = builder.Build();
    //app.Urls.Add("http://192.168.31.178:5050"); // Ubuntu IP
    //app.Urls.Add("http://localhost:5050"); // Localhost

// Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    throw ex;
}
finally
{
    NLog.LogManager.Shutdown();
}
