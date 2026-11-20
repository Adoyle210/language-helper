using Microsoft.OpenApi;
using Microsoft.EntityFrameworkCore;
using LanguageHelper.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WordDb>(options => options.UseInMemoryDatabase("items"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
     c.SwaggerDoc("v1", new OpenApiInfo {
         Title = "Languge Helper API",
         Description = "Helping you study your target languge",
         Version = "v1" });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI(c =>
   {
      c.SwaggerEndpoint("/swagger/v1/swagger.json", "Languge Helper API V1");
   });
}


app.MapGet("/", () => "Hello World!");

app.MapGet("/Words", async (WordDb db) => await db.Words.ToListAsync());

app.MapPost("/words", async (WordDb db, Word word) =>
{
    await db.Words.AddAsync(word);
    await db.SaveChangesAsync();
    return Results.Created($"/words/{word.Id}", word);
});

app.MapGet("/Words/{id}", async (WordDb db, int id) => await db.Words.FindAsync(id));

app.MapPut("/Words/{id}", async (WordDb db, Word updateword, int id) =>
{
      var word = await db.Words.FindAsync(id);
      if (word is null) return Results.NotFound();
      word.Stared = updateword.Stared;
      await db.SaveChangesAsync();
      return Results.NoContent();
});


app.MapDelete("/words/{id}", async (WordDb db, int id) =>
{
   var word = await db.Words.FindAsync(id);
   if (word is null)
   {
      return Results.NotFound();
   }
   db.Words.Remove(word);
   await db.SaveChangesAsync();
   return Results.Ok();
});


app.Run();
