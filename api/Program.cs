using api.Data;
using api.Services;
using api.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using api.HubsAll;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()) // Enable detailed logging only in development
           .LogTo(Console.WriteLine));   // Log to console

// Register EmailService for dependency injection
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<EmailService>();

// Register SignalR
builder.Services.AddSignalR();

// Add CORS to allow requests from your React Native app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware for different environments
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use static files middleware to serve files from wwwroot
app.UseStaticFiles();

// Use CORS policy to allow requests from different origins
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapHub<PostHub>("/postHub");
app.MapHub<CommentHub>("/commentHub");
app.MapControllers();

app.Run();