using CompanyVerification.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCompanyVerification(o =>
{
    o.Abr.Guid = builder.Configuration["ABR__Guid"] ?? string.Empty;
    o.Nzbn.SubscriptionKey = builder.Configuration["NZBN__SubscriptionKey"] ?? string.Empty;
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Render terminates TLS at the proxy. The app only receives plain HTTP.
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
