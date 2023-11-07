# ASP.NET-Core7-WebAPI-RedisCache
ASP .NET Core Project for learning. Every one should be able to use this templae to build a ASP .NET Core web API with PostgreSQL database and redis for cache.

### Key Functions 
1. Book's API (ADD, UPDATE, DELETE, GET)
2. Entity Framework Core
3. Swagger for API's Endpoint
4. API-Versioning
5. Serial-Log
6. AutoMapper
7. StackExchangeRedis
8. Newtonsoft.Json

## Getting Started
These instructions will get you to setup the project, install sdk and add package (CLI or Package manager console).

### Prerequisites
- Visual Studio 2022 or higher 
- .NET 7.x SDK  
- Npgsql.EntityFrameworkCore.PostgreSQL 7.0.11 (https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL/)

### Installing
1.  Install .net SDK 7<br>
[Download .NET SDK here.](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks)
2.  Create new Web API's project<br>
`dotnet new webapi –name API`
3.  Add package
     - Entity Framework Core 7.0.13<br>
       `dotnet add package Microsoft.EntityFrameworkCore -v 7.0.13`<br>
       `dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL -v 7.0.11`
     - API-Versioning
       `dotnet add package Asp.Versioning.Mvc.ApiExplorer -v 7.1.0`
     - Serial-Log
       `dotnet add package Serilog.AspNetCore`
     - AutoMapper
       `dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection`
     - StackExchangeRedis
        `dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis`
     - Newtonsoft.Json
       `dotnet add package Newtonsoft.Json`
4.  Migrate Model to Database<br>
     - Command Line<br>
      Add Migration `dotnet ef migrations add AddBookToDB`<br>
      Update to DB `dotnet ef database update`
     - Package Manager Console<br>
      Add Migration`add-migration AddBookToDB`<br>
      Update to DB `update-database`
## Languages and Tools
<div>
  <img src="https://github.com/devicons/devicon/blob/master/icons/dotnetcore/dotnetcore-original.svg" title="dotnet core" alt="dotnet core" width="40" height="40"/>&nbsp;
  <img src="https://codeopinion.com/wp-content/uploads/2017/10/Bitmap-MEDIUM_Entity-Framework-Core-Logo_2colors_Square_Boxed_RGB.png" title="dotnet core" alt="dotnet core" width="40" height="40"/>&nbsp;
  <img src="https://github.com/devicons/devicon/blob/master/icons/csharp/csharp-original.svg" title="csharp" alt="csharp" width="40" height="40"/>&nbsp;
  <img src="https://github.com/devicons/devicon/blob/master/icons/postgresql/postgresql-original.svg" title="postgresql" alt="postgresql" width="40" height="40"/>&nbsp;
  <img src="https://upload.wikimedia.org/wikipedia/en/thumb/6/6b/Redis_Logo.svg/1200px-Redis_Logo.svg.png" title="redis" alt="redis" width="80" height="40"/>&nbsp;
</div>
