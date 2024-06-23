using BusinessAccessLayer.Helper;
using KycBizWebApi.Helper;
using KycBizWebApi.Repository.Countries;
using KycBizWebApi.Repository.KycDoc;
using KycBizWebApi.Repository.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using DataAccessLayer.Repository.Common;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace KycBizWebApi
{

    public class Startup
    {

        public IConfiguration _configuration { get; }
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddEntityFrameworkSqlServer();
            var connDb = _configuration.GetConnectionString("KycBizDatabase");
            services.AddDbContextPool<DataAccessLayer.Models.KycbizContext>((serviceProvider, optionsBuilder) =>
            {
                optionsBuilder.UseSqlServer(connDb);
                optionsBuilder.UseInternalServiceProvider(serviceProvider);
            });
            

            services.AddCors(options =>
            {
                options.AddPolicy("KycBizWebApi",
                    builder =>
                    {
                        builder.WithOrigins() // Specify the allowed origins
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });

            //services.AddDbContextPool<DataAccessLayer.Models.KycbizContext>(options => options.UseSqlServer(connDb));
            services.AddLogging(builder =>
            {
                builder.AddConsole(); // Add console logger
                builder.AddDebug(); // Add debug logger
                // Add additional log providers as needed
            });
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "KycBiz CoreAPI", Version = "v1" });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
                };
            });

            services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = _configuration["Google:ClientId"];
                options.ClientSecret = _configuration["Google:ClientSecret"];
                options.CallbackPath = "/signin-google";
            });

            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IUserRespository, UserRespository>();
            services.AddScoped<IKycDocRepository, KycDocRepository>();
            services.AddScoped<ICommonRepository, CommonRepository>();
            services.AddScoped<HelperClass>();




            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory logFactory)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }

            //logFactory. = LogLevel.Information;

            //app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors("KycBizWebApi");
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


    }

}
