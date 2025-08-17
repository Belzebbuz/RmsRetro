using System.Globalization;
using RmsRetro.Silo.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((_, config) =>
{
	config.WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
		.MinimumLevel.Information();
});
builder.Host.AddOrleans();
var app = builder.Build();
app.Run();