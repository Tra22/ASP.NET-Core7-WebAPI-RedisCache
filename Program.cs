using APICacheWithRedis.Middleware.ExceptionHandler;
using APICacheWithRedis.Configuration;
using APICacheWithRedis.Repositories;
using APICacheWithRedis.Repositories.Interface;
using APICacheWithRedis.Services;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using APICacheWithRedis.Entities;
using StackExchange.Redis;

const string SwaggerRoutePrefix = "api-docs";

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<DataContext>
    (options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//redis cache config
builder.Services.AddStackExchangeRedisCache(options => {
    RedisConfigOptions? RedisConfigOptions = builder.Configuration.GetSection("Redis").Get<RedisConfigOptions>();
    if(RedisConfigOptions is not null){
        options.ConfigurationOptions = 
        RedisConfigOptions.Username is not null && RedisConfigOptions.Password is not null ?
        new ConfigurationOptions{
            EndPoints = { $"{RedisConfigOptions.Url}" },
            User = RedisConfigOptions.Username,
            Password = RedisConfigOptions.Password
        }: 
        ConfigurationOptions.Parse(RedisConfigOptions.Url);
    }
    // options.ConfigurationOptions = ConfigurationOptions.Parse(string.Format("{0}:{1},password={2}", tokens[0], tokens[3], tokens[1]));
});

builder.Services
    .AddApiVersioning(
                options =>
                {
                    // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                    options.ReportApiVersions = true;
                })
    .AddApiExplorer(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1,0);
        // Add the versioned API explorer, which also adds IApiVersionDescriptionProvider service
        // note: the specified format code will format the version as "'v'major[.minor][-status]"
        options.GroupNameFormat = "'v'VVV";

        // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
        // can also be used to control the format of the API version in route templates
        options.SubstituteApiVersionInUrl = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
    });
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    // Add a custom operation filter which sets default values
    options.OperationFilter<SwaggerDefaultValues>();
});

builder.Services.AddAutoMapper(typeof(Program));    
 //inject Data Access Layer - Repository
builder.Services.AddScoped<IBookRepository, BookRepository>();

   //inject Service layer
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddControllers();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => { options.RouteTemplate =  $"{SwaggerRoutePrefix}/{{documentName}}/docs.json";});
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = SwaggerRoutePrefix;
        foreach (var description in app.DescribeApiVersions())
            options.SwaggerEndpoint($"/{SwaggerRoutePrefix}/{description.GroupName}/docs.json", description.GroupName.ToUpperInvariant());
    });
}
app.UseSerilogRequestLogging();

//Middleware 
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
