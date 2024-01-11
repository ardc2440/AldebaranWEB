using Aldebaran.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder);
var app = builder.Build();
Configure(app);
app.Run();

static void ConfigureServices(WebApplicationBuilder builder)
{
    builder.AddArchitecture();
    builder.AddInfraestructure();
}
static void Configure(WebApplication app)
{
    ArgumentNullException.ThrowIfNull(app);
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseHeaderPropagation();
    //app.UseRequestLocalization(options => options.AddSupportedCultures("es-CO").AddSupportedUICultures("es-CO").SetDefaultCulture("es-CO"));
    app.UseRequestLocalization();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");
    //app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>().Database.Migrate();
}