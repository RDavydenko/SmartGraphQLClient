using HotChocolate.Types.Pagination;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SmartGraphQLClient.GraphQLServer.Database;
using SmartGraphQLClient.GraphQLServer.GraphQL;
using SmartGraphQLClient.GraphQLServer.Services;
using SmartGraphQLClient.GraphQLServer.Services.Abstractions;
using System.Text;

namespace SmartGraphQLClient.GraphQLServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            Configure(app);

            DatabaseMock.SeedDatabase();

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors();
            services.AddAuthorization();
            services.AddControllers();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                var config = configuration.GetSection("Jwt");
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = config["Issuer"],
                    ValidAudience = config["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Key"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true
                };
            });

            services.AddScoped<IUnitOfWork, MockUnitOfWork>();

            services
               .AddGraphQLServer()
               .AddQueryType<Query>()
               .AddType<AuthorizedQuery>()
               .SetPagingOptions(new PagingOptions { DefaultPageSize = 20, IncludeTotalCount = true })
               .AddAuthorization()
               .AddProjections()
               .AddFiltering()
               .AddSorting();
        }

        private static void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(c => c
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGraphQL();
            });
        }
    }
}