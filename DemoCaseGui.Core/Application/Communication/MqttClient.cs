using DemoCaseGui.Core.Application.Exceptions;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using System.Timers;
using System.Windows;

namespace DemoCaseGui.Core.Application.Communication;
public class MqttClient
{
    public MqttOptions Options { get; set; }
    public bool IsConnected => _mqttClient is not null && _mqttClient.IsConnected;

    public event Func<MqttApplicationMessageReceivedEventArgs, Task>? ApplicationMessageReceived;

    private IMqttClient? _mqttClient;

    private CancellationTokenSource _cancellationTokenSource;
    public MqttClient()
    {
        _mqttClient = new MqttFactory().CreateMqttClient();
        Options = new MqttOptions()
        {
            CommunicationTimeout = 30,
            Host = "40.82.154.13",
            Port = 1883,
            KeepAliveInterval = 10
        };
        
    }

    public async Task ConnectAsync()
    {
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(Options.Host, Options.Port)
            .WithTimeout(TimeSpan.FromSeconds(Options.CommunicationTimeout))
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(Options.KeepAliveInterval));

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        _mqttClient.ApplicationMessageReceivedAsync += ApplicationMessageReceived;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(Options.CommunicationTimeout));
        try
        {
            var result = await _mqttClient.ConnectAsync(mqttClientOptions.Build(), timeout.Token);

            if (result.ResultCode != MqttClientConnectResultCode.Success)
            {
                throw new MqttConnectionException($"{result.ResultCode}: {result.ReasonString}");
            }
        }
        catch (Exception ex)
        {
            await DisconnectAsync();
            MessageBox.Show(ex.Message,"Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
    }

    public async Task DisconnectAsync()
    {
       await _mqttClient.DisconnectAsync();
    }

    
    public async Task Subscribe(string topic)
    {


#pragma warning disable CS8602 // Dereference of a possibly null reference.
        if (_mqttClient.IsConnected)
        {
            var topicFilter = new MqttTopicFilterBuilder()
           .WithTopic(topic)
           .Build();

            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(topicFilter)
                .Build();

            try
            {
                var result = await _mqttClient.SubscribeAsync(subscribeOptions);
                foreach (var subscription in result.Items)
                {
                    if (subscription.ResultCode != MqttClientSubscribeResultCode.GrantedQoS0 &&
                        subscription.ResultCode != MqttClientSubscribeResultCode.GrantedQoS1 &&
                        subscription.ResultCode != MqttClientSubscribeResultCode.GrantedQoS2)
                    {
                        Console.WriteLine($"MQTT Client Subscription {subscription.TopicFilter.Topic} Failed: {subscription.ResultCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisconnectAsync();
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }
        
#pragma warning restore CS8602 // Dereference of a possibly null reference.




    }

    public async Task Publish(string topic, string payload, bool retainFlag)
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        if  (_mqttClient.IsConnected)
        {
            var applicationMessageBuilder = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithRetainFlag(retainFlag)
            .WithPayload(payload);

            var applicationMessage = applicationMessageBuilder.Build();

            try
            {
                var result = await _mqttClient.PublishAsync(applicationMessage);

                if (result.ReasonCode != MqttClientPublishReasonCode.Success)
                {
                    Console.WriteLine($"MQTT Client Publish {applicationMessage.Topic} Failed: {result.ReasonCode}");
                }
            }
            catch (Exception ex)
            {   await DisconnectAsync();
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;                 
            }
        }
        else
        {
            //throw new InvalidOperationException("MQTT Client is not connected.");
            MessageBox.Show("MQTT Client is not connected.");

        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.


    }

}
