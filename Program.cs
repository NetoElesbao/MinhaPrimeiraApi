using Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Settings;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);

var app = builder.Build();
var appConfiguration = app.Configuration;
ProductRepository.Init(app.Configuration);

app.MapPost("/products/", (ProductRequest productRequest, ApplicationDbContext context) =>
{
    var category = context.Categories.Where(c => c.Id.Equals(productRequest.CategoryId)).First();
    var product = new Product
    {
        Code = productRequest.Code,
        Name = productRequest.Name,
        Description = productRequest.Description,
        Category = category
    };

    if (productRequest.Tags != null)
    {
        product.Tags = new List<Tag>();

        foreach (var item in productRequest.Tags)
        {
            product.Tags.Add(new Tag { Name = item });
        }
    }
    context.Products.Add(product);
    context.SaveChanges();
    return Results.Created($"/products/{product.Id}", product.Id);
});

app.MapGet("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
{
    var product = context.Products
        .Include(p => p.Category)
        .Include(p => p.Tags)
        .Where(p => p.Id.Equals(id)).FirstOrDefault();

    if (product != null)
    {
        return Results.Ok(product);
    }
    else
    {
        return Results.NotFound();
    }
});

app.MapPut("/products/{id}", ([FromRoute] int id, ProductRequest productRequest, ApplicationDbContext context) =>
{
    var product = context.Products
        .Include(p => p.Tags)
        .Where(p => p.Id.Equals(id)).First();

    var category = context.Categories.Where(c => c.Id.Equals(productRequest.CategoryId)).First();

    product.Code = productRequest.Code;
    product.Name = productRequest.Name;
    product.Description = productRequest.Description;
    product.Category = category;

    if (productRequest.Tags != null)
    {
        product.Tags = new List<Tag>();
        foreach (var item in productRequest.Tags)
        {
            product.Tags.Add(new Tag { Name = item });
        }
    }

    context.SaveChanges();
    return Results.Ok();
});

app.MapDelete("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
{
    var product = context.Products.Where(p => p.Id.Equals(id)).First();
    context.Products.Remove(product);
    context.SaveChanges();
    return Results.Ok();
});

app.Run();












// if (app.Environment.IsStaging())
//     app.MapGet("/configuration/database/", (IConfiguration configuration) =>
//     {
//         return Results.Ok($"{configuration["Database:Connection"]}/{configuration["Database:Port"]}");
//     });



