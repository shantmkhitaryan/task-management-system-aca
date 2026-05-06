using task_management_system_aca.Data;
using task_management_system_aca.Services;

var builder = WebApplication.CreateBuilder(args);

// Connection string
var connectionString = "Host=localhost;Port=5432;Database=taskmanagement;Username=admin;Password=admin123";

// Create database factory
var dbConnectionFactory = new NpgsqlConnectionFactory(connectionString);

// Register services one by one
builder.Services.AddSingleton<IDbConnectionFactory>(dbConnectionFactory);
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<BoardService>();
builder.Services.AddSingleton<SectionService>();
builder.Services.AddSingleton<TaskService>();

// Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();