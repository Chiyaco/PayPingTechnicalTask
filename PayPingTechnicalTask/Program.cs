using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using PayPingTechnicalTask.Data;
using PayPingTechnicalTask.Service;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<TransactionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseLazyLoadingProxies());


// enable Cors
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin",
        options => options.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

// Json Serializer
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
    .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver
    = new DefaultContractResolver());

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region dependencies

builder.Services.AddTransient<HttpClient>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(ITransactionService), typeof(TransactionService));

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    TransactionDbContext dbContext = scope.ServiceProvider.GetRequiredService<TransactionDbContext>();

    dbContext.Database.EnsureCreated();
    dbContext.Seed(app).GetAwaiter().GetResult();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
