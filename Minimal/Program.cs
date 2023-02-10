
using Microsoft.EntityFrameworkCore;
using Minimal.Data;
using Minimal.Models;
using MiniValidation;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//configuração do contexto
builder.Services.AddDbContext<MinimalContextDb>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
    app.UseSwagger();
}




//endpoint Get fornecedores 
app.MapGet("/fornecedor", async (MinimalContextDb context) =>
    await context.Fornecedores.ToListAsync())
    .Produces<Fornecedor[]>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("GetFornecedor")
    .WithTags("Fornecedor");
//================================================================================================

//endpoint Get fornecedores por id 
app.MapGet("/fornecedor/{id}", async (Guid id, MinimalContextDb context) =>
    await context.Fornecedores.FindAsync(id)
        is Fornecedor fornecedor
            ? Results.Ok(fornecedor)
            : Results.NotFound())
    .Produces<Fornecedor>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("GetFornecedorPorId")
    .WithTags("Fornecedor");
//================================================================================================

//endpoint Post fornecedor
app.MapPost("/fornecedor", async (Fornecedor fornecedor, MinimalContextDb context) =>
{
    //validação do objeto fornecedor
    if (!MiniValidator.TryValidate(fornecedor, out var errors))    
    {
        return Results.ValidationProblem(errors);
    }

    context.Fornecedores.Add(fornecedor);
    var result = await context.SaveChangesAsync();

    return result > 0 
       ? Results.Created($"/fornecedor/{fornecedor.Id}", fornecedor) 
        : Results.BadRequest("Houve um problema ao adicionar o fornecedor");

    
})
    .ProducesValidationProblem()
    .Produces<Fornecedor>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("PostFornecedor")
    .WithTags("Fornecedor"); ;
//================================================================================================

//endpoint Put fornecedor
app.MapPut("/fornecedor/{id}", async (Guid id, Fornecedor fornecedor, MinimalContextDb context) =>
{
    //validação do guid fornecedor
    var fornecedorBanco = await context.Fornecedores.AsNoTracking<Fornecedor>()
                                                    .FirstOrDefaultAsync(f => f.Id == id);

    if (fornecedorBanco == null)
    {
        return Results.NotFound();
    }
    
    //validação do objeto fornecedor
    if (!MiniValidator.TryValidate(fornecedor, out var errors))
    {
        return Results.ValidationProblem(errors);
    }

    context.Fornecedores.Update(fornecedor);
    var result = await context.SaveChangesAsync();

    return result > 0
       ? Results.NoContent()
        : Results.BadRequest("Houve um problema ao atualizar o fornecedor");


})
    .ProducesValidationProblem()
    .Produces<Fornecedor>(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("PutFornecedor")
    .WithTags("Fornecedor"); ;
//================================================================================================

//endpoint delete fornecedor
app.MapDelete("/fornecedor/{id}", async (Guid id, MinimalContextDb context) =>
{
    //validação do guid fornecedor
    var fornecedorBanco = await context.Fornecedores.FindAsync(id);
    if (fornecedorBanco == null)
    {
        return Results.NotFound();
    }

    context.Fornecedores.Remove(fornecedorBanco);
    var result = await context.SaveChangesAsync();

    return result > 0
       ? Results.NoContent()
        : Results.BadRequest("Houve um problema ao deletar o fornecedor");


})    
    .Produces<Fornecedor>(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("DeleteFornecedor")
    .WithTags("Fornecedor"); ;
//================================================================================================



app.Run();
