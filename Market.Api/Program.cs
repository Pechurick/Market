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


var app = builder.Build();

// Глобальний перехоплювач помилок
app.Use(async (context, next) =>
{
    try
    {
        await next(); // Пропускаємо запит далі в контролери
    }
    catch (ValidationException ex) // 👈 НОВИЙ БЛОК ДЛЯ ВАЛІДАЦІЇ
    {
        context.Response.StatusCode = 400; // Bad Request
        context.Response.ContentType = "application/json";
        
        // Збираємо всі помилки валідації у зручний масив для фронтенду
        var errors = ex.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage });
        
        await context.Response.WriteAsJsonAsync(new { 
            error = "Помилка валідації даних", 
            details = errors 
        });
    }
    catch (ArgumentNullException ex) // Якщо чогось не знайдено
    {
        context.Response.StatusCode = 404; // Not Found
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "Ресурс не знайдено", details = ex.ParamName });
    }
    catch (InvalidOperationException ex) // Якщо порушена бізнес-логіка (як з видаленням категорії)
    {
        context.Response.StatusCode = 400; // Bad Request
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "Помилка бізнес-логіки", message = ex.Message });
    }
    catch (Exception ex) // Усі справжні аварії
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
