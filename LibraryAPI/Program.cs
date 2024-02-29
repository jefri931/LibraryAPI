using LibraryAPI;
using LibraryAPI.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var services = builder.Services;
services.AddLogging(b => b.AddSerilog());
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddControllers();
services.AddAutoMapper(config => config.AddProfile<MappingProfile>());
services.AddLibraryAPIDbContext(builder.Configuration);
services.AddLibraryJwtAuth(builder.Configuration);
services.AddLibraryServices();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1"));
app.UseHttpLogging();
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});
app.Run();
