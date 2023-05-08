//using Fluent.Infrastructure.FluentModel;
using Microsoft.EntityFrameworkCore;
using texnotest.Repositories;
//using AutoMapper;



namespace texnotest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            
                // Получение строки подключения к базе данных из конфигурации
                var connectionString = Configuration.GetConnectionString("DefaultConnection");

                // Регистрация DbContext для работы с базой данных PostgreSQL
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(connectionString));

                // Регистрация сервиса для работы с пользователем
                services.AddScoped<IUserService, UserService>();

                // Регистрация сервиса для работы с группами пользователей
                services.AddScoped<IUserGroupService, UserGroupService>();

                // Регистрация сервиса для работы со статусами пользователей
                services.AddScoped<IUserStateService, UserStateService>();

                // Регистрация AutoMapper для маппинга между классами моделей и DTO
                services.AddAutoMapper(typeof(Startup));
                services.AddControllers();
                services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // конфигурация конвейера обработки запросов
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}");
            });
        }
    }
}