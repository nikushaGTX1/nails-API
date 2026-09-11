using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi;
using NailsApi.Data;
using NailsApi.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString=builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

builder.Services.AddControllers();
builder.Services.AddDbContext<NailsDbContext>(options => options.UseNpgsql(
    connectionString,
    npgsql => npgsql.ConfigureDataSource(dataSource => dataSource.EnableDynamicJson())));
builder.Services.AddScoped<IContentService,ContentService>();
builder.Services.AddScoped<AdminCredentialService>();
builder.Services.AddSingleton<AdminSessionService>();
builder.Services.AddSingleton<MediaStorageService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",new OpenApiInfo { Title="Nail Bar CMS API",Version="v1",Description="Content management API for Nail Bar 01." });
    options.AddSecurityDefinition("Bearer",new OpenApiSecurityScheme { Type=SecuritySchemeType.Http,Scheme="bearer",BearerFormat="Token",Description="Paste the token returned by POST /api/admin/login." });
});
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
    .WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? ["http://localhost:4200"])
    .AllowAnyHeader().AllowAnyMethod()));

Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath,"wwwroot","uploads"));
var app=builder.Build();

app.UseExceptionHandler(error => error.Run(async context =>
{
    var exception=context.Features.Get<IExceptionHandlerFeature>()?.Error;
    app.Logger.LogError(exception,"Unhandled exception while processing {Method} {Path}",context.Request.Method,context.Request.Path);
    context.Response.StatusCode=StatusCodes.Status500InternalServerError;
    context.Response.ContentType="application/problem+json";
    await Results.Problem("The server could not complete the request.",statusCode:500).ExecuteAsync(context);
}));
app.UseCors();
app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json","Nail Bar CMS API v1"); options.DocumentTitle="Nail Bar CMS API"; options.DisplayRequestDuration(); });
}
app.MapControllers();
await app.InitializeAsync();
app.Run();

public partial class Program;
