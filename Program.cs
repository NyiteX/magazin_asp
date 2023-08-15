using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;
using System;
using System.Text;
using WebApplicationDB1;
using WebApplicationDB1.Models;

var builder = WebApplication.CreateBuilder(args);

var people = new List<Person>
{
    new Person("qwe123@gmail.com", "12345"),
    new Person("zxc123@gmail.com", "55555")
};
/*string connection = "Server=(localdb)\\mssqllocaldb;Database=applicationdb2;Trusted_Connection=True;";*/
/*string connection = builder.Configuration.GetConnectionString("DefaultConnection");*/

string connection = @"Data Source = WIN-U669V8L9R5E; Initial Catalog = Tovars123; Trusted_Connection=True; TrustServerCertificate = True";
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => options.LoginPath = "/login");
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/login", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    string loginForm = @"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>Document</title>
</head>
<body>
  <h2>Login Form</h2>
  <form method=""post"">
    <p>
      <label>Email:</label>
      <input type=""email"" name=""email""/>
    </p>
    <p>
      <label>Password:</label>
      <input type=""password"" name=""password""/>
    </p>
    <input type=""submit"" value=""Login""/>
  </form>
</body>
</html>";
    await context.Response.WriteAsync(loginForm);
    await Console.Out.WriteLineAsync("-------------------LOGIN_GET");
    /*context.Response.Redirect("/login.html");
    return Task.CompletedTask;*/
});


app.MapPost("/login", async (string returnUrl, HttpContext context) =>
{
    await Console.Out.WriteLineAsync("-------------------LOGIN_POST");
    var form = context.Request.Form;

    if (!form.ContainsKey("email") || !form.ContainsKey("password"))
        return Results.BadRequest("Email или Password не установлен");

    string email = form["email"];
    string password = form["password"];

    Person person = people.FirstOrDefault(p => p.Email == email && p.Password == password);

    if (person is null)
        return Results.Unauthorized();

    var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Email) };

    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");

    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
    return Results.Redirect(returnUrl ?? "/");
});


app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});


app.Map("/", [Authorize] (ctx) =>
{
    ctx.Response.Redirect("/index.html");
    return Task.CompletedTask;
});

app.MapGet("/api/tovars", async (ApplicationContext db) =>
{
    await Console.Out.WriteLineAsync("DBALL.................................... !!!!!!!!!!!!!!!!!!!");

    /*await db.Tovars.ToListAsync();*/
    List<TovarTMP> tovarTMPs = new List<TovarTMP>();
    foreach (var item in db.Tovars)
    {
        var url = Convert.ToBase64String(item.Url);
        tovarTMPs.Add(new TovarTMP(item.Id,item.Name,item.Discription,item.Price,item.Count, url));
        
    }
    return Results.Json(tovarTMPs);
});

app.MapGet("/api/tovars/{id:int}", async (int id, ApplicationContext db) =>
{
    Tovar? user = await db.Tovars.FirstOrDefaultAsync(u => u.Id == id);

    if (user == null)
        return Results.NotFound(new { message = "Товар не найден" });

    return Results.Json(user);
});

app.MapDelete("/api/tovars/{id:int}", async (int id, ApplicationContext db) =>
{
    Tovar? user = await db.Tovars.FirstOrDefaultAsync(u => u.Id == id);

    if (user == null)
        return Results.NotFound(new { message = "Товар не найден" });

    db.Tovars.Remove(user);
    await db.SaveChangesAsync();
    return Results.Json(user);
});

app.MapPost("/api/tovars/",  async (HttpContext context, ApplicationContext db) =>
{
    var tovartmp = await context.Request.ReadFromJsonAsync<TovarTMP>();

    Tovar tovar = new Tovar();
    if (tovartmp.Url != null && tovartmp.Url.Length > 0)
    {
        byte[] decodedByteArray = Convert.FromBase64String(tovartmp.Url);
        tovar.Id = tovartmp.Id;
        tovar.Name = tovartmp.Name;
        tovar.Discription = tovartmp.Discription;
        tovar.Price = tovartmp.Price;
        tovar.Count = tovartmp.Count;
        tovar.Url = decodedByteArray;
    }

    await db.Tovars.AddAsync(tovar);
    await db.SaveChangesAsync();

    return Results.Json(tovar);
});

app.MapPut("/api/tovars",  async (Tovar userData, ApplicationContext db) =>
{
    Tovar? user = await db.Tovars.FirstOrDefaultAsync(u => u.Id == userData.Id);

    if (user == null)
        return Results.NotFound(new { message = "Товар не найден" });

    user.Id = userData.Id;
    user.Name = userData.Name;
    user.Discription = userData.Discription;
    user.Price = userData.Price;
    user.Count = userData.Count;
    user.Url = userData.Url;
    await db.SaveChangesAsync();
    return Results.Json(user);
});

/*app.MapFallback(ctx =>
{
    ctx.Response.Redirect("/index.html");
    return Task.CompletedTask;
});*/

app.Run();


record class TovarTMP(int Id,string Name, string Discription,double Price, long Count, string Url);
record class Person(string Email, string Password);