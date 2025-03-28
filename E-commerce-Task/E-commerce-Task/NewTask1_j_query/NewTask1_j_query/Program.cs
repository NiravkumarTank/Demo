//using Microsoft.Extensions.FileProviders;
//using NewTask1_j_query.Repository.Service;
//using NewTask1_j_query.Repository.Interface;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.


//builder.Services.AddCors(option =>
//{
//    option.AddDefaultPolicy(policy =>
//    {
//        policy.AllowAnyOrigin()
//          .AllowAnyHeader()
//          .AllowAnyMethod();

//    });
//});

//builder.Services.AddControllers();

//// Add Session and Memory Cache services
//builder.Services.AddDistributedMemoryCache(); // For storing session data in memory
//builder.Services.AddSession(options =>
//{
//    options.Cookie.HttpOnly = true;  // Cookie cannot be accessed via JavaScript
//    options.Cookie.IsEssential = true;  // The session cookie is essential for the app to function
//    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Session expires after 30 minutes of inactivity
//});

//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
//builder.Services.AddTransient<IEmailSender, EmailSenders>();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();


//app.UseCors();
//app.UseSession();

//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Upload")),
//    RequestPath = "/Upload"
//});

//app.MapControllers();

//app.Run();



using Microsoft.Extensions.FileProviders;
using NewTask1_j_query.Repository.Service;
using NewTask1_j_query.Repository.Interface;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// CORS Configuration
builder.Services.AddCors(option =>
{
    option.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular front-end URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Allow cookies to be sent with cross-origin requests
    });
});

// Add controllers to the container
builder.Services.AddControllers();

// Add Session and Memory Cache services
builder.Services.AddDistributedMemoryCache(); // For storing session data in memory
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;  // Cookie cannot be accessed via JavaScript
    options.Cookie.IsEssential = true;  // The session cookie is essential for the app to function
    options.Cookie.SameSite = SameSiteMode.None; // Allow cross-origin cookie sending
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Only send cookies over HTTPS
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Session expires after 30 minutes of inactivity
});

// Add Email service
builder.Services.AddTransient<IEmailSender, EmailSenders>();

// Add Swagger services (optional, for API documentation)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS

// Apply CORS policy
app.UseCors("AllowSpecificOrigin"); // Ensure that the CORS policy is applied

// Session middleware - Make sure session is before Authorization
app.UseSession();

// Authorization middleware (comes after session and CORS)
app.UseAuthorization();

// Static file configuration for serving uploaded files
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Upload")),
    RequestPath = "/Upload"
});

// Map controllers
app.MapControllers();

// Run the application
app.Run();
