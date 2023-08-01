using LearnIndentityAndAuthorization.Configs;
using LearnIndentityAndAuthorization.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Add AutoMapper
builder.Services.AddAutoMapperExtension();

// Add Identity
builder.Services.AddIdentityExtension(builder.Configuration);

// Add DI
builder.Services.AddDependencyInjectionExtension();

// Add DbContext
builder.Services.AddDbContextExtension(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerExtension();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add Seed Data
app.AddSeedDataExtension();

app.UseHttpsRedirection();

app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
    options.AllowAnyHeader();
});

app.UseAuthentication();

app.UseAuthorization();


// Add Exception Handler
app.HandleExceptionAsync();

app.AddStaticFileExtension();

app.MapControllers();

app.Run();
