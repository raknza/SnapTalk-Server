using android_backend.Helper;
using android_backend.Models;
using android_backend.Repositories;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using MQTTnet.Client;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;

namespace android_backend.Mqtt
{
    public class MqttService
    {
        private readonly MqttServer _mqttServer;
        private readonly IServiceProvider _serviceProvider;
        private UserRepository _repository;
        private MessageRepository messageRepository;
        
        IMqttClient mqttClient;
        public MqttService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var options = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(1883)
                .Build();
            _mqttServer = new MqttFactory().CreateMqttServer(options);
            _mqttServer.ValidatingConnectionAsync += ValidateConnectionAsync;

            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();

        }

        private Task HandleMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs args){
            
            var receivedMessage = args.ApplicationMessage;
            var payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
            
            try
            {
                var message = JsonConvert.DeserializeObject<Message>(payload);
                messageRepository.Create(message);
                
            }
            catch (Newtonsoft.Json.JsonException)
            {
                Console.WriteLine("Failed to deserialize payload.");
            }

            return Task.CompletedTask;
        }

        private Task ValidateConnectionAsync(ValidatingConnectionEventArgs args)
        {
            _repository = _serviceProvider.GetRequiredService<UserRepository>();
            messageRepository = _serviceProvider.GetRequiredService<MessageRepository>();
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
            if(mqttClient.IsConnected == false){
                MqttClientOptions clientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer("127.0.0.1", 1883)
                    .WithClientId("MyClientId")
                    .WithCleanSession()
                    .WithCredentials("root", MD5Helper.Hash("rootroot"))
                    .Build();
                ConnectAsync(clientOptions).Wait();
            }
        }

        private async Task ConnectAsync(MqttClientOptions options)
        {
            await mqttClient.ConnectAsync(options);
            await mqttClient.SubscribeAsync("message/#");
            mqttClient.ApplicationMessageReceivedAsync += HandleMessageReceivedAsync;
        }

        public async Task PublishMessageAsync(string topic, string payload)
        {
            MqttApplicationMessage message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(Encoding.UTF8.GetBytes(payload)) 
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
                .WithRetainFlag(false)
                .Build();
            await mqttClient.PublishAsync(message);
        }
    }


}