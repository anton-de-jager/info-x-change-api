//using infoX.api.Data;
//using infoX.api.Services;
//using infoX.api.Models;            // ← for LoginRequest

//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Mvc;              // ← add this
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;             // ← for Swagger
//using System.Text;
//using infoX.api.Hubs;
//using Microsoft.Extensions.Options;

//var builder = WebApplication.CreateBuilder(args);

//// ─── 1) Logging ────────────────────────────────────────────────────────────
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();

//// ─── 2) JWT Authentication ─────────────────────────────────────────────────
//var jwtSettings = builder.Configuration.GetSection("Jwt");
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//  .AddJwtBearer(opts =>
//  {
//      opts.TokenValidationParameters = new TokenValidationParameters
//      {
//          ValidateIssuer = true,
//          ValidateAudience = true,
//          ValidateLifetime = true,
//          ValidateIssuerSigningKey = true,
//          ValidIssuer = jwtSettings["Issuer"],
//          ValidAudience = jwtSettings["Audience"],
//          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
//      };
//      // allow SignalR auth via query string
//      opts.Events = new JwtBearerEvents
//      {
//          OnMessageReceived = ctx =>
//          {
//              var accessToken = ctx.Request.Query["access_token"];
//              var path = ctx.HttpContext.Request.Path;
//              if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/api/chatHub"))
//                  ctx.Token = accessToken;
//              return Task.CompletedTask;
//          }
//      };
//  });
//builder.Services.AddAuthorization();

//// ─── 3) CORS ───────────────────────────────────────────────────────────────
//builder.Services.AddCors(o =>
//  o.AddPolicy("AllowAngularDevClient", p =>
//    p.WithOrigins
//    (
//        "http://localhost:4200",
//        "https://localhost:4200",
//        "https://infox.adei.co.za",
//        "https://infox.local:5002"
//        )
//     .AllowAnyHeader()
//     .AllowAnyMethod()
//     .AllowCredentials()
//));

//// ─── 4) EF Core ────────────────────────────────────────────────────────────
//builder.Services.AddDbContext<AppDbContext>(o =>
//  o.UseSqlServer(builder.Configuration.GetConnectionString("DataWarehouseConnection")));
//builder.Services.AddDbContext<PegasusUserDbContext>(o =>
//  o.UseSqlServer(builder.Configuration.GetConnectionString("PegasusConfigurationConnection")));

//// ─── 5) Application Services ──────────────────────────────────────────────
//builder.Services.AddTransient<IEmailService, EmailService>();

//// SignalR
//builder.Services.AddSignalR();

//// WhatsApp service
//builder.Services.AddHttpClient<IWhatsAppService, WhatsAppService>();

//// ─── 6) Facebook Integration ──────────────────────────────────────────────
//// Bind config
//builder.Services.Configure<FacebookSettings>(
//    builder.Configuration.GetSection("Facebook"));

//// HttpClient for Facebook calls
//builder.Services.AddHttpClient<IFacebookService, FacebookService>();

//// ─── 7) MVC + Validation + Swagger ────────────────────────────────────────
//builder.Services
//  .AddControllers()
//  .ConfigureApiBehaviorOptions(opts =>
//  {
//      opts.InvalidModelStateResponseFactory = ctx =>
//      {
//          var errors = ctx.ModelState
//                          .Values
//                          .SelectMany(v => v.Errors)
//                          .Select(e => e.ErrorMessage);

//          // ← use BadRequestObjectResult (IActionResult) instead of Results.BadRequest (IResult)
//          return new BadRequestObjectResult(new { errors });
//      };
//  });
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new()
//    {
//        Title = "Facebook Message Templates API",
//        Version = "v1",
//        Description = "Retrieves Facebook WhatsApp message templates."
//    });
//});

//// ─── 8) Kestrel / URLs ─────────────────────────────────────────────────────
//builder.WebHost.ConfigureKestrel((ctx, opts) =>
//  opts.Configure(ctx.Configuration.GetSection("Kestrel")));
//var urlConfig = builder.Configuration["urls"];
//if (!string.IsNullOrEmpty(urlConfig))
//{
//    var urls = urlConfig
//      .Split(';', StringSplitOptions.RemoveEmptyEntries);
//    builder.WebHost.UseUrls(urls);
//}

//var app = builder.Build();

//// ─── 9) Middleware Pipeline ───────────────────────────────────────────────
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.RoutePrefix = "adei_meta_templates";  // matches your custom path
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Templates API V1");
//    });
//}
//else
//{
//    app.UseExceptionHandler("/error");
//}

//app.UseRouting();
//app.UseCors("AllowAngularDevClient");
//app.UseAuthentication();
//app.UseAuthorization();

//// If you host your Angular SPA here:
//app.UseDefaultFiles();
//app.UseStaticFiles();

//// ─── 10) Endpoints ─────────────────────────────────────────────────────────
//app.MapControllers();
//app.MapHub<ChatHub>("/api/chatHub");
//app.MapGet("/", () => Results.Ok(new { status = "API is up!" }));
//app.MapFallbackToFile("index.html");

//// Facebook templates endpoint
//app.MapGet("/api/templates", async (IFacebookService svc) =>
//{
//    var result = await svc.GetTemplatesAsync();
//    return Results.Ok(result);
//})
//.WithName("GetMessageTemplates")
//.WithTags("templates")
//.Produces<TemplateResponse>(200)
//.Produces<ApiError>(400)
//.Produces<ApiError>(401)
//.Produces<ApiError>(403)
//.Produces<ApiError>(500);

//app.Run();

//// ─── 11) Strongly‑typed settings ───────────────────────────────────────────
//public record FacebookSettings(string WabaId, string AccessToken);

//// Error model
//public record ApiError(string Message, object? Details);

//// Template response (adjust fields to match FB JSON)
//public record TemplateResponse(object? Data, object? Paging);

//// Service interface
//public interface IFacebookService
//{
//    Task<TemplateResponse> GetTemplatesAsync();
//}

//// Service implementation
//public class FacebookService : IFacebookService
//{
//    private readonly HttpClient _http;
//    private readonly FacebookSettings _settings;

//    public FacebookService(HttpClient http, IOptions<FacebookSettings> options)
//    {
//        _http = http;
//        _settings = options.Value;
//    }

//    public async Task<TemplateResponse> GetTemplatesAsync()
//    {
//        if (string.IsNullOrWhiteSpace(_settings.WabaId))
//            throw new InvalidOperationException("WABA_ID not configured.");

//        if (string.IsNullOrWhiteSpace(_settings.AccessToken))
//            throw new InvalidOperationException("ACCESS_TOKEN not configured.");

//        var url = $"https://graph.facebook.com/v19.0/{_settings.WabaId}/message_templates";
//        using var req = new HttpRequestMessage(HttpMethod.Get, url);
//        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _settings.AccessToken);

//        var res = await _http.SendAsync(req);
//        var payload = await res.Content.ReadFromJsonAsync<TemplateResponse>();

//        if (res.IsSuccessStatusCode && payload != null)
//            return payload;

//        // Try to read error from FB
//        var errorObj = await res.Content.ReadFromJsonAsync<object>();
//        var msg = $"Facebook API returned {(int)res.StatusCode}";
//        throw new HttpRequestException(msg, null, res.StatusCode);
//    }
//}

using infoX.api.Data;
using infoX.api.Services;
using infoX.api.Models;            // ← for LoginRequest

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;              // ← add this
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using infoX.api.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ─── 1) Logging ────────────────────────────────────────────────────────────
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ─── 2) JWT Authentication ────────────────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
  .AddJwtBearer(opts =>
  {
      opts.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = jwtSettings["Issuer"],
          ValidAudience = jwtSettings["Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
      };
      // allow SignalR auth via query string
      opts.Events = new JwtBearerEvents
      {
          OnMessageReceived = ctx =>
          {
              var accessToken = ctx.Request.Query["access_token"];
              var path = ctx.HttpContext.Request.Path;
              if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/api/chatHub"))
                  ctx.Token = accessToken;
              return Task.CompletedTask;
          }
      };
  });
builder.Services.AddAuthorization();

// ─── 3) CORS ───────────────────────────────────────────────────────────────
builder.Services.AddCors(o =>
  o.AddPolicy("AllowAngularDevClient", p =>
    p.WithOrigins
    (
        "http://localhost:4200",
        "https://localhost:4200",
        "https://infox.adei.co.za",
        "https://infox.local:5002"
        )
     .AllowAnyHeader()
     .AllowAnyMethod()
     .AllowCredentials()
));

// ─── 4) EF Core ────────────────────────────────────────────────────────────
builder.Services.AddDbContext<PegasusDataWarehouseDbContext>(o =>
  o.UseSqlServer(builder.Configuration.GetConnectionString("DataWarehouseConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null)));
builder.Services.AddDbContext<PegasusDataLakeDbContext>(o =>
  o.UseSqlServer(builder.Configuration.GetConnectionString("DataLakeConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null)));
builder.Services.AddDbContext<PegasusDataMartDbContext>(o =>
  o.UseSqlServer(builder.Configuration.GetConnectionString("DataMartConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null)));
builder.Services.AddDbContext<WhatsAppDbContext>(o =>
  o.UseSqlServer(builder.Configuration.GetConnectionString("WhatsAppConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null)));
builder.Services.AddDbContext<PegasusConfigurationDbContext>(o =>
  o.UseSqlServer(builder.Configuration.GetConnectionString("PegasusConfigurationConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null)));

// ─── 5) Application Services ──────────────────────────────────────────────
builder.Services.AddTransient<IEmailService, EmailService>();

// SignalR
builder.Services.AddSignalR();

// WhatsApp service
builder.Services.AddHttpClient<IWhatsAppService, WhatsAppService>();

// ─── 6) MVC + Validation + Swagger ────────────────────────────────────────
builder.Services
  .AddControllers()
  .ConfigureApiBehaviorOptions(opts =>
  {
      opts.InvalidModelStateResponseFactory = ctx =>
      {
          var errors = ctx.ModelState
                          .Values
                          .SelectMany(v => v.Errors)
                          .Select(e => e.ErrorMessage);

          // ← use BadRequestObjectResult (IActionResult) instead of Results.BadRequest (IResult)
          return new BadRequestObjectResult(new { errors });
      };
  });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ─── 7) Kestrel / URLs ─────────────────────────────────────────────────────
builder.WebHost.ConfigureKestrel((ctx, opts) =>
  opts.Configure(ctx.Configuration.GetSection("Kestrel")));
var urlConfig = builder.Configuration["urls"];
if (!string.IsNullOrEmpty(urlConfig))
{
    var urls = urlConfig
      .Split(';', StringSplitOptions.RemoveEmptyEntries);
    builder.WebHost.UseUrls(urls);
}

var app = builder.Build();

// ─── 8) Middleware Pipeline ───────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseRouting();
app.UseCors("AllowAngularDevClient");
app.UseAuthentication();
app.UseAuthorization();

// If you host your Angular SPA here:
app.UseDefaultFiles();
app.UseStaticFiles();

// ─── 9) Endpoints ─────────────────────────────────────────────────────────
app.MapControllers();
app.MapHub<ChatHub>("/api/chatHub");
app.MapGet("/", () => Results.Ok(new { status = "API is up!" }));
app.MapFallbackToFile("index.html");

app.Run();
