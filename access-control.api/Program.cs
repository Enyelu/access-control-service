using access_control.api.Extensions;
using access_control.api.Jobs.Interfaces;
using access_control.api.Middlewares;
using access_control.core;
using access_control.infrastructure;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Serilog;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureApplicationServices(builder.Configuration);
builder.Services.ConfigureApplicationDatabase(builder.Configuration);
builder.Services.AddApplicationCore();
builder.Services.ConfigureHangfireSettings(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.InjectFluentValidations();

// Configure logger
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .WriteTo.Seq(context.Configuration["Serilog:WriteTo:0:Args:serverUrl"]));


var app = builder.Build();
Log.Information("Application is starting - Access control");

// Configure the HTTP request pipeline.

app.UseCors("AllowAll");

app.UseHangfireDashboard("/hangfire");
app.UseHangfireServer();
// Schedule recurring job
RecurringJob.AddOrUpdate<ILockJobs>(job => job.CheckAndCloseOpenDoors(), Cron.MinuteInterval(5));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Log errors or handle exceptions
        Console.WriteLine(ex.Message);
    }
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuditMiddleware>();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
