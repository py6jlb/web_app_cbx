using cookbook.Extensions;
using cookbook.Features;
using cookbook.Features.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();

var app = builder.Build();

if (app.Environment.IsProduction())
{
    app.UseStatusCodePagesWithRedirects("/StatusCode/{0}");
    app.UseExceptionHandler("/StatusCode/500", true);
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseAntiforgery();

app.MapGet("/", () => new RazorComponentResult<Home>());
app.MapGet("/success-alert", () => new RazorComponentResult<SuccessAlert>());   
app.Map("/statuscode/{code:int}", (int code) => new RazorComponentResult<StatusCode>(new { code }));

app.MapApplicationRoutes();

app.Run();
