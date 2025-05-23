using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Dodanie niezbędnych usług
builder.Services.AddControllersWithViews();

// Konfiguracja sesji
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Konfiguracja klientów HTTP dla mikroserwisów
builder.Services.AddHttpClient("BookService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5002/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient("OrderService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5004/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient("UserService", client =>
{
    // Poprawny adres do UsereService - dopasuj port do swojej konfiguracji
    client.BaseAddress = new Uri("http://localhost:5018/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Dodanie HttpContextAccessor dla dostępu do sesji
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Konfiguracja middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Dodanie middleware sesji PRZED autoryzacją
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();