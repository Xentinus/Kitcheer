using Microsoft.EntityFrameworkCore;
using Kitcheer.API.Data;
using Kitcheer.API.Functions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Swagger/OpenAPI konfiguráció
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Kitcheer API",
        Version = "v1",
        Description = "API a Kitcheer alkalmazáshoz - kamra, hűtő és fagyasztó készletkezelés",
    Contact = new OpenApiContact
   {
       Name = "Kitcheer Team",
   Email = "support@kitcheer.com"
      }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.UseInlineDefinitionsForEnums();
});

// PostgreSQL adatbázis kapcsolat konfigurálása
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<StorageLocationFunctions>();
builder.Services.AddScoped<ProductTemplateFunctions>();
builder.Services.AddScoped<StoredProductFunctions>();
builder.Services.AddScoped<ShoppingListFunctions>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
 var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
 
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
   
  await context.Database.MigrateAsync();
      
      logger.LogInformation("Adatbázis migrációk sikeresen alkalmazva");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Hiba történt az adatbázis migrációk alkalmazása során");
    }
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Application-Name", "Kitcheer");
    context.Response.Headers.Append("X-Application-Version", "1.0");
    context.Response.Headers.Append("X-Application-Description", "Kitchen pantry and storage management system");
    
    await next();
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kitcheer API v1");
  c.RoutePrefix = "swagger";
    c.DocumentTitle = "Kitcheer API Documentation";
  c.DefaultModelsExpandDepth(2);
    c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Example);
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
