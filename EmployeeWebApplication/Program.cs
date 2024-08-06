using ManagementAPI.Contract.Interfaces;
using ManagementAPI.Provider.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.Text;

namespace ManagementAPI.Provider.Database;



public class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();

        }));


        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))

        };
    });


        //// Add role-based authorization
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
           options.AddPolicy("SuperAdminPolicy", policy => policy.RequireRole("SuperAdmin"));
           options.AddPolicy("EmployeePolicy", policy => policy.RequireRole("Employee"));
        });

        // Database Connectivity
        var provider = builder.Services.BuildServiceProvider();
        var config = provider.GetRequiredService<IConfiguration>();

        builder.Services.AddControllers();

        // Add services to the container.bearer
        builder.Services.AddDbContext<dbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("connectionString")));

        builder.Services.AddScoped<IEmployeeServices, EmployeeServices>();
        builder.Services.AddScoped<IDepartmentServices, DepartmentServices>();
        builder.Services.AddScoped<ITasksServices, TasksService>();
        builder.Services.AddScoped<ITaskReviewServices, TaskReviewServices>();
        builder.Services.AddScoped<IAttendenceServices, AttendenceServices>();
        builder.Services.AddScoped<ISalaryServices, SalaryServies>();
        builder.Services.AddScoped<IProjectServices, ProjectServices>();
        builder.Services.AddScoped<IAuthServices, AuthServices>();
        builder.Services.AddScoped<IPaginatedService,PaginatedServices>();
    



        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        //
        builder.Services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "EmployeeManagementAPI", Version = "v1" });
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
            opt.OperationFilter<CustomHeaderSwaggerAttribute>();

        });
        //
        

        var app = builder.Build();
        // Configure the HTTP request pipeline.
        //app.UseDeveloperExceptionPage();
        app.UseCors("MyPolicy");

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
        app.UseHttpsRedirection();
       
        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();


        
        app.MapControllers();

        app.Run();

    }
    



}

