var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/services/developers/outages", () =>
{
    var thanksGiving = new DateTime(2022, 11, 24);
    var thanksGivingOutage = new ScheduledOutage(thanksGiving, thanksGiving.AddDays(1), "Party Time");
    var response = new { data = new List<ScheduledOutage>() { thanksGivingOutage } };
    return Results.Ok(response);
});

app.Run();

public record ScheduledOutage(DateTime startTime, DateTime endTime, string reason);