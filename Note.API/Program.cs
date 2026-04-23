using Microsoft.EntityFrameworkCore;
using NoteDatabase.DbContext;
using NotesServices;
using NotesServices.Interfaces;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<NoteDatabase.Abstraction.INoteRepository, NoteDatabase.Repositories.Repository>();
builder.Services.AddScoped<INoteService, NotesService>();
builder.Services.AddDbContext<NoteDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(NoteDbContext)));
    }
    
    );



var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
   
}

app.UseAuthorization();

app.MapControllers();

app.Run();
