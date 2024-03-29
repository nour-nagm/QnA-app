using DbUp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using QnA.Api.Authorization;
using QnA.Api.Data;

namespace QnA.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            #region DbUp Migration
            EnsureDatabase.For.SqlDatabase(connectionString);

            // Create and configure an instance of the DbUp upgrader
            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString, schema: null)
                .WithScriptsEmbeddedInAssembly(
                    System.Reflection.Assembly.GetExecutingAssembly())
                .WithTransaction()
                .LogToConsole()
                .Build();

            // Do a database migration if there are any pending SQL scripts
            if (upgrader.IsUpgradeRequired())
            {
                upgrader.PerformUpgrade();
            }
            #endregion

            services.AddScoped<IDataRepository, DataRepository>();

            services.AddMemoryCache();
            services.AddSingleton<IQuestionCache, QuestionCache>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "QnA.Api", Version = "v1" });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = Configuration["Auth0:Authority"];
                options.Audience = Configuration["Auth0:Audience"];
            });

            services.AddHttpClient();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("MustBeQuestionAuthor", policy =>
                {
                    policy.Requirements.Add(new MustBeQuestionAuthorRequirement());
                });
            });

            services.AddScoped<IAuthorizationHandler, MustBeQuestionAuthorHandler>();

            services.AddHttpContextAccessor();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins(Configuration["Frontend"]);
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "QnA.Api v1"));
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
