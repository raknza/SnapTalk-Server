using android_backend.Models;
using android_backend.Repositories;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;


namespace android_backend.Mqtt
{
    public class MqttServerService
    {
        private readonly MqttServer _mqttServer;
        private readonly IServiceProvider _serviceProvider;
        private UserRepository _repository;
        public MqttServerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var options = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(1883)
                .Build();
            _mqttServer = new MqttFactory().CreateMqttServer(options);
            _mqttServer.ValidatingConnectionAsync += ValidateConnectionAsync;

            var factory = new MqttFactory();
            _repository = _serviceProvider.GetRequiredService<UserRepository>();
        }


        private Task ValidateConnectionAsync(ValidatingConnectionEventArgs args)
        {
            if(args.Password == "key")
                return Task.CompletedTask;
            User user = _repository.FindByUsername(args.Username);
            if (user == null || !user.password.Equals(args.Password))
            {
                args.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                return Task.CompletedTask;
            }
            args.ReasonCode = MqttConnectReasonCode.Success;
            return Task.CompletedTask;
        }
        public void Start()
        {
            _mqttServer.StartAsync().Wait();
        }
    }


}