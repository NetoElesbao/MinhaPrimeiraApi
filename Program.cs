using Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;

var builder = WebApplication.CreateBuilder();
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);

var app = builder.Build();

app.MapPost("/products/", ([FromBody] ProductRequest request, ApplicationDbContext context) =>
{
    var product = new Product
    {
        Code = request.Code,
        Name = request.Name,
        Description = request.Description,
        CategoryId = request.CategoryId
    };
    
    foreach (var item in request.Tags)
        product.Tags.Add(new Tag { Name = item });

    context.Products.Add(product);
    context.SaveChanges();

    return Results.Created("/product/", product.Id);
});

app.MapGet("/products/", (ApplicationDbContext context) =>
{
    var products = context.Products
     .Include(c => c.Category)
     .Include(t => t.Tags)
     .Select(p => p).ToList();

    if (products.Any())
        return Results.Ok(products);

    return Results.NotFound("Não há produtos!");
});

app.MapPut("/products/{id}", ([FromRoute] int id, [FromBody] ProductRequest request, ApplicationDbContext context) =>
{
    var product = context.Products
    .Include(p => p.Tags)
    .Include(p => p.Category)
    .Where(p => p.Id.Equals(id)).First();

    if (product is not null)
    {
        product.Code = request.Code;
        product.Name = request.Name;
        product.Description = request.Description;
        product.CategoryId = request.CategoryId;

        product.Tags = new List<Tag>();
        if (request.Tags is not null)
            foreach (var item in request.Tags)
                product.Tags.Add(new Tag { Name = item });

        context.SaveChanges();
        return Results.Ok("Atualizado com sucesso!");
    }
    return Results.NotFound("Não encontrado!");
});

app.MapDelete("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
{
    var product = context.Products.Where(p => p.Id.Equals(id)).First();

    if (product is not null)
    {
        context.Remove(product);
        context.SaveChanges();
        return Results.Ok("Deletado com sucesso!");
    }
    return Results.NotFound("Não encontrado!");
});

app.Run();