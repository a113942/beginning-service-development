using DevelopersApi.Adapters;
using DevelopersApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<OutageSupplierHttpAdapter>(httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration.GetConnectionString("outages-api"));
}).AddPolicyHandler(BasicPolicies.GetRetyPolicy()).AddPolicyHandler(BasicPolicies.GetCircutBreakerPolicy());

builder.Services.AddSingleton<MongoDevelopersAdapter>((sp) =>
{
    return new MongoDevelopersAdapter(builder.Configuration.GetConnectionString("mongo"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
