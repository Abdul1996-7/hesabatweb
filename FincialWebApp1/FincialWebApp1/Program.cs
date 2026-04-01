using FincialWebApp1.Models;
using FincialWebApp1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using FincialWebApp1.Security;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Security"),
    b=>b.CommandTimeout(360)
    ));
builder.Services.AddDbContext<FincialWebApp1Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FincialWebApp1Context") ?? throw new InvalidOperationException("Connection string 'FincialWebApp1Context' not found."),
    b=>b.CommandTimeout(300)
    ));
builder.Services.AddScoped<ImainClass, mainclassDetails>();



builder.Services.AddMvc(options =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.ConfigureApplicationCookie(options =>
options.AccessDeniedPath = new PathString("/Administrator/AccessDenied"));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(Options =>
{
    Options.Password.RequiredLength = 4;
    Options.Password.RequireDigit = false;
}).AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DeleteRolePolicy",
        policy => policy.RequireClaim("Delete Role"));

    options.AddPolicy("EditRolePolicy", policy =>
           policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
    options.AddPolicy("AdminRolePolicy",
       policy => policy.RequireRole("Admin"));
});
builder.Services.AddSingleton<IAuthorizationHandler,
       CanEditOnlyOtherAdminRolesAndClaimsHandler>();
builder.Services.AddMvc().AddXmlSerializerFormatters();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
