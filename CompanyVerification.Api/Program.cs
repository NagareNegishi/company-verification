using CompanyVerification.Core;
using CompanyVerification.Core.Providers.Au;
using CompanyVerification.Core.Providers.Nz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();

// reads ABR__Guid from env
builder.Services.Configure<AbrOptions>(builder.Configuration.GetSection("ABR"));

// reads NZBN__SubscriptionKey from env
builder.Services.Configure<NzbnOptions>(builder.Configuration.GetSection("NZBN"));

// routing layer picks the right adapter at runtime
builder.Services.AddSingleton<IVerificationProvider, AbrProvider>();
builder.Services.AddSingleton<IVerificationProvider, NzbnProvider>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
