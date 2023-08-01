using LearnIndentityAndAuthorization.Databases;
using LearnIndentityAndAuthorization.Models;
using LearnIndentityAndAuthorization.Repositories;
using LearnIndentityAndAuthorization.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using LearnIndentityAndAuthorization.Controllers.Filters;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace LearnIndentityAndAuthorization.Configs;

public static class Extensions
{
    /// <summary>
    /// Thêm AutoMapper vào dự án
    /// </summary>
    /// <param name="services"></param>
    public static void AddAutoMapperExtension(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Program));
    }
    /// <summary>
    /// Thêm Identity và JWT vào dự án
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddIdentityExtension(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Identity
        services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddErrorDescriber<VietnameseIdentityErrorDescriber>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            // Add JWT
            .AddJwtBearer(options =>
            {
                var key = configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(key))
                {
                    throw new Exception("Chưa cấu hình khoá bảo mật");
                }

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                // options.TokenValidationParameters = new TokenValidationParameters()
                // {
                //     ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256Signature },
                //     ValidateActor = true,
                //     ValidateIssuer = true,
                //     ValidateAudience = true,
                //     RequireExpirationTime = true,
                //     ValidateIssuerSigningKey = true,
                //     ValidIssuer = configuration["Jwt:Issuer"],
                //     ValidAudience = configuration["Jwt:Audience"],
                //     IssuerSigningKey = new SymmetricSecurityKey(Encoding
                //     .UTF8.GetBytes(key))
                // };
            })
            // // Add Cookie
            // .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            // Add Oauth Google
            .AddGoogle(options =>
            {
                string? clientId = configuration["Google:ClientId"];
                string? clientSecret = configuration["Google:ClientSecret"];
                options.ClientId = clientId ?? throw new Exception("Chưa cấu hình ClientId của Google");
                options.ClientSecret = clientSecret ?? throw new Exception("Chưa cấu hình ClientSecret của Google");
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.SaveTokens = true;
                options.Scope.Add("email");
                options.Scope.Add("profile");
                options.AccessType = "offline";
                options.TokenEndpoint = "https://accounts.google.com/o/oauth2/token";
                options.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
            });
    }
    /// <summary>
    /// Thêm Swagger vào dự án
    /// </summary>
    /// <param name="services"></param>
    public static void AddSwaggerExtension(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            // thêm thông tin cho swagger
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "LearnIdentityAndAuthorizationAPI",
                Version = "v1"
            });

            // loại trừ các route không cần yêu cầu bảo mật
            options.OperationFilter<RemoveUnnecessaryApiFilter>();

            // thêm file upload cho swagger
            options.OperationFilter<SwaggerFileUploadFilter>();

            // thêm bảo mật cho swagger
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
        });
    }
    /// <summary>
    /// Thêm các dịch vụ cần thiết cho dự án
    /// </summary>
    /// <param name="services"></param>
    public static void AddDependencyInjectionExtension(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<UserManager<ApplicationUser>>();
        services.AddScoped<RoleManager<IdentityRole>>();

        services.AddScoped<IRoleRepository, RoleRepository>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IPostService, PostService>();
    }

    public static void AddDbContextExtension(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("MSSQL"));
        });
    }
    /// <summary>
    /// Thêm dữ liệu mẫu vào dự án
    /// </summary>
    /// <param name="app"></param>
    public static void AddSeedDataExtension(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        ApplicationDbContextSeeder.SeedData(context).Wait();
    }
    /// <summary>
    /// Thêm đường dẫn thư mục tĩnh cho dự án
    /// </summary>
    /// <param name="app"></param>
    public static void AddStaticFileExtension(this IApplicationBuilder app)
    {
        var folderName = Path.Combine("Resources");
        var staticPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        if (!Directory.Exists(staticPath))
        {
            Directory.CreateDirectory(staticPath);
        }

        var options = new StaticFileOptions
        {
            FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(staticPath),
            RequestPath = "/Resources"
        };

        app.UseStaticFiles(options);
    }
}
