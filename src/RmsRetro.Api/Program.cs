using System.Globalization;
using RmsRetro.Api.Extensions;
using RmsRetro.Api.Grpc;
using RmsRetro.Api.Grpc.Interceptors;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, config) =>
{
	config.WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
		.MinimumLevel.Information();
});
builder.Host.AddOrleansClient(builder.Configuration);
builder.Services.AddDefaultCorsPolicy("default");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGrpc(options =>
{
	options.Interceptors.Add<ExceptionHandlingInterceptor>();
	options.Interceptors.Add<OrleansMetadataInterceptor>();
}).AddJsonTranscoding();
builder.Services.AddGrpcReflection();
builder.Services.AddControllers();
var app = builder.Build();

app.UseRouting();
app.UseCors("default");
app.UseGrpcWeb(new GrpcWebOptions()
{
	DefaultEnabled = true
});
app.UseStaticFiles();
app.MapFallbackToFile("index.html").AllowAnonymous();
app.MapGrpcService<ApiGrpcService>().RequireCors("default");
app.MapGrpcReflectionService();
app.MapControllers();
app.Run();