using Microsoft.EntityFrameworkCore;
using MinimalShoppingListApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApiDbContext>(opt => opt.UseInMemoryDatabase("ShoppingListAPI"));

var app = builder.Build();

app.MapGet("/shoppingList", async (ApiDbContext db) =>
 await db.Groceries.ToListAsync());

app.MapPost("/shoppingList", async (Grocery grocery, ApiDbContext db) =>
{
    db.Groceries.Add(grocery);
    await db.SaveChangesAsync();
    return Results.Created($"/shoppingList/{grocery.Id}", grocery);
}
);

app.MapGet("/shoppingList/{id}",async (int id,ApiDbContext db)=>{
    var result = await db.Groceries.FindAsync(id);
    return result!=null ? Results.Ok(result):Results.NotFound();
});

app.MapDelete("/shoppingList/{id}", async (int id, ApiDbContext db) =>
{
    var grocery = await db.Groceries.FindAsync(id);
    if (grocery != null)
    {
        db.Groceries.Remove(grocery);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.MapPut("/shoppingList/{id}", async (int id, Grocery item, ApiDbContext db) =>
{
    var grocery = await db.Groceries.FindAsync(id);

    if (grocery != null)
    {
        grocery.Name = item.Name;
        grocery.Purchased = item.Purchased;
        await db.SaveChangesAsync();
        return Results.Ok(grocery);
    }

    return Results.NotFound();

});
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();