global using Microsoft.AspNetCore.Mvc;
global using Newtonsoft.Json;
global using System;
global using System.Linq;
global using System.Net;
using Unidas.MS.Maintenance.CheckinCheckout.API.Helpers;
using Unidas.MS.Maintenance.CheckinCheckout.Application.Interfaces.Services;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels;
using Unidas.MS.Maintenance.CheckinCheckout.Application.ViewModels.Requests;
using Unidas.MS.Maintenance.CheckinCheckout.Infra.IoC;
using Microsoft.OpenApi.Models;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region Configuracoes adicionadas - builder.services
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("V1", new OpenApiInfo() { Title = "API V1", Version = "V1.0" });
    //options.SwaggerDoc("V2", new OpenApiInfo() { Title = "API V2", Version = "V2.0" });
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    options.CustomSchemaIds(x => x.FullName);
});

NativeInjector.RegisterServices(builder.Services);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

builder.Services.AddMvc(options =>
{
    options.Filters.Add(typeof(ValidateActionFilterAttribute));
});

var appSettings = new AppSettings();
builder.Configuration.Bind("AppSettings", appSettings);
builder.Services.AddSingleton(appSettings);
#endregion

var app = builder.Build();

#region Configuracoes adicionadas - app

app.UseMiddleware(typeof(ApiExceptionMiddleware));
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint($"/swagger/V1/swagger.json", "V1.0");
    });
}

app.UseHttpsRedirection();


#region Endpoints


app.MapPost("/integrate", async (ItemCheckinCheckoutRequestViewModel request, ICheckinCheckoutService service) =>
{
    app.Logger.LogInformation($"Integração de Checkin/Checkout", request);

    return Results.Ok(await service.Integrate(request));

});


#endregion


app.Run();