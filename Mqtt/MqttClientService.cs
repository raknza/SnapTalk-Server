using android_backend.Models;
using android_backend.Repositories;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using MQTTnet.Client;
using System.Text;
using Newtonsoft.Json;

namespace android_backend.Mqtt
{
    public class MqttClientService
    {
        IMqttClient mqttClient;
        private MessageRepository messageRepository;
        IServiceProvider serviceProvider;
        
        public MqttClientService(IServiceProvider serviceProvider)
        {
            if(this.serviceProvider == null){
                this.serviceProvider = serviceProvider;
                var factory = new MqttFactory();
                mqttClient = factory.CreateMqttClient();
            }
        }

        private Task HandleMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs args){
            var receivedMessage = args.ApplicationMessage;
            var payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
            var scope = serviceProvider.CreateScope();
            messageRepository = scope.ServiceProvider.GetRequiredService<MessageRepository>();
            if(receivedMessage.Topic.Equals("message/received")){
                List<Message> messages = messageRepository.FindByUserUsername(payload);
                for(int i=0;i<messages.Count;i++){
                    messages[i].isReceived = true;
                    messageRepository.Update(messages[i]);
                }
                return Task.CompletedTask;
            }
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

        public void Start(String clientId = "defaultClientId")
        {
            if(mqttClient.IsConnected == false){
                MqttClientOptions clientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer("127.0.0.1", 1883)
                    .WithClientId(clientId)
                    .WithCleanSession()
                    .WithCredentials("root", "key")
                    .Build();
                Connect(clientOptions);
            }
        }

        private void Connect(MqttClientOptions options)
        {
            mqttClient.ConnectAsync(options).Wait();
        }
        public void Subscribe(){
            mqttClient.SubscribeAsync("message/#").Wait();
            mqttClient.ApplicationMessageReceivedAsync += HandleMessageReceivedAsync;
        }

        public void PublishMessage(string topic, string payload)
        {
            if(mqttClient.IsConnected == false){
                Start();
            }
            
            MqttApplicationMessage message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(Encoding.UTF8.GetBytes(payload))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
                .WithRetainFlag(false)
                .Build();
            mqttClient.PublishAsync(message).Wait();
        }
    }


}