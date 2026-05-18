using Market.Application;
using Market.Infrastructure;
using FluentValidation;
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(Market.Application.Commands.CreateOrderCommand).Assembly));


var app = builder.Build();


app.Use(async (context, next) =>
{
    try
    {
        await next(); 
    }
    catch (ValidationException ex) 
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";
        
        
        var errors = ex.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage });
        
        await context.Response.WriteAsJsonAsync(new { 
            error = "Помилка валідації даних", 
            details = errors 
        });
    }
    catch (ArgumentNullException ex) 
    {
        context.Response.StatusCode = 404; 
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "Ресурс не знайдено", details = ex.ParamName });
    }
    catch (InvalidOperationException ex) 
    {
        context.Response.StatusCode = 400; 
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "Помилка бізнес-логіки", message = ex.Message });
    }
    catch (Exception ex) 
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "Внутрішня помилка сервера", message = ex.Message });
    }
});

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();

await app.RunAsync();
