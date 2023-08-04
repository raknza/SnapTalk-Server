using android_backend.Helper;
using android_backend.Models;
using android_backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace android_backend
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<UserRepository, UserRepository>();
            services.AddSingleton<JwtHelper>();
            services.BuildServiceProvider().GetService<JwtHelper>().addService(services);
            services.AddDbContextPool<MyDbContext>(options =>
            {
                options.UseMySql(_config.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(_config.GetConnectionString("MySqlVersion"))));
            });



        }
    }
}