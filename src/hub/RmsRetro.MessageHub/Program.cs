using System.Globalization;
using RmsRetro.MessageHub.Extensions;
using RmsRetro.MessageHub.Grpc;
using RmsRetro.MessageHub.Options;
using RmsRetro.MessageHub.Services;
using RmsRetro.Protos.Api;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((_, config) =>
{
	config.WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
		.MinimumLevel.Information();
});
builder.Services.AddControllers();

builder.Services.AddHubCors();
builder.Services.AddServices();
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcReflection();
var apiSetting = builder.Configuration.GetSection(nameof(ApiOptions)).Get<ApiOptions>()
                      ?? throw new ArgumentNullException(nameof(ApiOptions));
builder.Services.AddGrpcClient<ApiService.ApiServiceClient>(o =>
{
	o.Address = new Uri(apiSetting.ConnectionString);
});
var app = builder.Build();
app.UseRouting();
app.UseCors("default");
app.UseGrpcWeb(new GrpcWebOptions()
{
	DefaultEnabled = true
});
app.MapGrpcService<HubApiGrpcService>().RequireCors("default");
app.MapGrpcReflectionService();
app.Run();