using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseExceptionHandler(a => a.Run(async (context) => await context.Response
.WriteAsJsonAsync(new {
    error = context.Features.Get<IExceptionHandlerPathFeature>().Error.Message
})));


app.UseCors("default");
//app.UseMiddleware<ErrorHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demp API - v1");
        c.OAuthClientId(builder.Configuration["AzureAd:ClientId"]);
        c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
        if (!builder.Environment.IsDevelopment())
            c.RoutePrefix = String.Empty;
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) => {
    if (!context.User.Identity?.IsAuthenticated ?? false)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Not Authenticated");
    }
    else await next();
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "WeatherForecast",
        pattern: "{area:exists}/{controller=WeatherForecast}/{action=list}"
    );

    endpoints.MapControllerRoute(
        name:"default",
        pattern:"{controller=Token}/{action=Get}"
    );
});

app.Run();

