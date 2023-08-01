var builder = WebApplication.CreateBuilder(args);
var startup = new android_backend.Startup(builder.Configuration);



// Add services to the container.
builder.Services.AddControllers();
startup.ConfigureServices(builder.Services);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();
//app.UseMvcWithDefaultRoute();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();





