using android_backend.Helper;
using android_backend.Mqtt;
using android_backend.Models;
using android_backend.Repositories;
using android_backend.Service;
using Microsoft.EntityFrameworkCore;
using android_backend.Filter;

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

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddScoped<UserRepository, UserRepository>();
            services.AddScoped<ContactRepository, ContactRepository>();
            services.AddScoped<MessageRepository, MessageRepository>();
            services.AddScoped<UserService, UserService>();
            services.AddScoped<ContactService, ContactService>();
            services.AddScoped<MessageService, MessageService>();
            services.AddScoped<JwtAuthenticationFilter>();
            services.AddSingleton<EnvHelper>();
            services.AddSingleton<JwtHelper>();
            services.AddSingleton<RedisService>();
            services.BuildServiceProvider().GetService<JwtHelper>().AddService(services);
            services.AddDbContextPool<MyDbContext>(options =>
            {
                options.UseMySql(_config.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(_config.GetConnectionString("MySqlVersion"))));
            });
            services.AddScoped<MqttServerService>();
            services.AddScoped<MqttClientService>();
            var mqttServerServiceProvider = services.BuildServiceProvider();
            var mqttServerService = mqttServerServiceProvider.GetRequiredService<MqttServerService>();
            var mqttClientService = mqttServerServiceProvider.GetRequiredService<MqttClientService>();
            mqttServerService.Start();
            mqttClientService.Start("start");
            mqttClientService.Subscribe();
            
        }
    }
}