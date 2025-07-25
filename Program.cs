using conj_ai.Middleware;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterGenericHandlers = true;
});
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("ip", configure =>
    {
        configure.PermitLimit = 2;
        configure.Window = TimeSpan.FromMinutes(1);
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();
app.UseMiddleware<SimpleExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
else
{
    app.UseRateLimiter();
}
app.MapControllers();
app.Run();