using Serilog;
using WizardAssessment.API.Configuration;
using WizardAssessment.API.Middleware;
using WizardAssessment.Application.Configuration;
using WizardAssessment.Domain.Configuration;
using WizardAssessment.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog(lc => lc
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console()
        .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "log/log.txt"), rollingInterval: RollingInterval.Day));

// Configure services by layer
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddDomain();
builder.Services.AddApplication();
builder.Services.AddApiServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
