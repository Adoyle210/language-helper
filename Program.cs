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

/****************************************************************************
* Endpoints: POST 
*            POST /words
****************************************************************************/

app.MapPost("/words", async (WordDb db, CreateWordRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Text))
        return Results.BadRequest("Text is required.");
    
    var text = request.Text.Trim().ToLowerInvariant();
    var language = request.Language ?? "es";

    // Have we saved this word before?
    var word = await db.Words.FirstOrDefaultAsync(w => w.Text == text && w.Language == language);
    var isNew = word is null;

    if (word is null)
    {
        word = new Word
        {
            Text = text,
            Translation = request.Translation,
            Language = language,
            Card = new Card()      // every new word gets its flashcard
        };
        db.Words.Add(word);
    }

    // Record this sighting, whether the word is new or not
    if (!string.IsNullOrWhiteSpace(request.Sentence))
    {
        db.Captures.Add(new Capture { Word = word, Sentence = request.Sentence.Trim() });
    }

    await db.SaveChangesAsync();

    var response = new { word.Id, word.Text, word.Translation, word.Language };
    return isNew
        ? Results.Created($"/words/{word.Id}", word)
        : Results.Ok(response);
});

/****************************************************************************
* Endpoints: GET
*            GET /words   
*            GET /words{id}
****************************************************************************/

app.MapGet("/words", async (WordDb db, string? language, bool? starred) =>
{
    var query = db.Words.AsQueryable();

    if (!string.IsNullOrWhiteSpace(language))
    {
        var lang = language.Trim().ToLowerInvariant();
        query = query.Where(w => w.Language == lang);
    } 
    if (starred is not null)
    {
        query = query.Where(w => w.Starred == starred);
    }

    return await query
        .Select(w => new { w.Id, w.Text, w.Translation, w.Language, w.Starred })
        .ToListAsync();
});

app.MapGet("/words/{id}", async (WordDb db, int id) => 
{
    var word = await db.Words
        .Include(w => w.Card)
        .Include(w => w.Captures)
        .FirstOrDefaultAsync(w => w.Id == id);

    return word is null
        ? Results.NotFound()
        : Results.Ok(new
        {
            word.Id, word.Text, word.Translation, word.Language,
            Due = word.Card?.DueDate,
            Captures = word.Captures.Select(c => new { c.Id, c.Sentence, c.CapturedAt })
        });
});


/****************************************************************************
* Endpoints: PUT 
*            PUT /words{id}
****************************************************************************/
app.MapPut("/words/{id}", async (WordDb db, Word updateword, int id) =>
{
      var word = await db.Words.FindAsync(id);
      if (word is null) return Results.NotFound();
      word.Starred = updateword.Starred;
      word.Translation =updateword.Translation;
      await db.SaveChangesAsync();
      return Results.NoContent();
});

/****************************************************************************
* Endpoints: DELETE 
*            DELETE /words{id}
****************************************************************************/
app.MapDelete("/words/{id}", async (WordDb db, int id) =>
{
   var word = await db.Words
        .Include(w => w.Card)
        .Include(w => w.Captures)
        .FirstOrDefaultAsync(w => w.Id == id);
  
   if (word is null)
   {
      return Results.NotFound();
   }

   db.Words.Remove(word);
   await db.SaveChangesAsync();
   return Results.Ok();
});


app.Run();
