using android_backend.Helper;
using android_backend.Models;
using android_backend.Repositories;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace android_backend.Mqtt
{
    public class MqttService
    {
        private readonly MqttServer _mqttServer;
        private readonly IServiceProvider _serviceProvider;
        private UserRepository _repository;
        public MqttService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            // set mqtt server 
            var options = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(1883)
                .Build();

            _mqttServer = new MqttFactory().CreateMqttServer(options);
            _repository = serviceProvider.GetRequiredService<UserRepository>();
            _mqttServer.ValidatingConnectionAsync += ValidateConnectionAsync;
        }

        private Task ValidateConnectionAsync(ValidatingConnectionEventArgs args)
        {
            User user = _repository.FindByUsername(args.Username);
            if (user == null || !user.password.Equals(MD5Helper.hash(args.Password)))
            {
                args.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                return Task.CompletedTask;
            }

            args.ReasonCode = MqttConnectReasonCode.Success;
            return Task.CompletedTask;
        }
        private async Task InitializeAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                _repository = scope.ServiceProvider.GetRequiredService<UserRepository>();

            }
        }
        public void Start()
        {
            _mqttServer.StartAsync().GetAwaiter().GetResult();
        }
    }


}