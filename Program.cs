using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using MvcFeatureFlags.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("sqlite");
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connectionString));

builder.Services.AddFeatureManagement() //Pra usar o FeatureFlag
	.AddFeatureFilter<PercentageFilter>() //Usando Filters
	.AddFeatureFilter<TimeWindowFilter>();

var app = builder.Build();

CreateDatabase(app);

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//app.UseMiddlewareForFeature<CorsMiddleware>(FeatureFlags.MiddlewareTeste); //Usando FeatureFlags por middleware

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static void CreateDatabase(WebApplication app)
{
	var serviceScope = app.Services.CreateScope();
	var dataContext = serviceScope.ServiceProvider.GetService<AppDbContext>();
	dataContext?.Database.EnsureCreated();
}