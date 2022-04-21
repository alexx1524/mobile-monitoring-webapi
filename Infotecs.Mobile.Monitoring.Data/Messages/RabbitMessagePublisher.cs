using System.Text;
using Infotecs.Mobile.Monitoring.Core.Messages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Core.DependencyInjection.Exceptions;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;

namespace Infotecs.Mobile.Monitoring.Data.Messages;

/// <summary>
/// Класс для публикации сообщений в шину данных.
/// </summary>
public class RabbitMessagePublisher : IMessagePublisher
{

    private readonly ILogger<RabbitMessagePublisher> logger;
    private readonly IProducingService producingService;

    /// <summary>
    /// Создание нового экземпляра класса.
    /// </summary>
    /// <param name="logger">Интерфейс логгирования.</param>
    /// <param name="producingService">Интерфейс для отправки сообщений в RabbitMQ.</param>
    public RabbitMessagePublisher(ILogger<RabbitMessagePublisher> logger, IProducingService producingService)
    {
        this.logger = logger;
        this.producingService = producingService;
    }

    /// <summary>
    /// Отправить сообщение об успешной обработке ивента от ноды.
    /// </summary>
    /// <param name="message">Объект сообщения об обработанном ивенте.</param>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
    public async Task SendProcessedNodeEventAsync(ProcessedNodeEventMessage message)
    {
        try
        {
            byte[] body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy()},
                Formatting = Formatting.None,
            }));

            IModel? channel = producingService.Channel;
            IBasicProperties properties = channel?.CreateBasicProperties() ?? throw new ChannelIsNullException();

            properties.ContentType = "application/json";
            properties.Persistent = true;

            await producingService.SendAsync(body, properties, "NodeEventsExchange", "processed");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка отправки сообщения об успешной обработке ивента {@Message}", message);
        }
    }

    /// <summary>
    /// Отправить сообщение об ошибке обработки ивента от ноды.
    /// </summary>
    /// <param name="message">Объект сообщения о необработанном ивенте.</param>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
    public async Task SendUnprocessedNodeEventAsync(UnprocessedNodeEventMessage message)
    {
        try
        {
            byte[] body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy()},
                Formatting = Formatting.None,
            }));

            IModel? channel = producingService.Channel;
            IBasicProperties properties = channel?.CreateBasicProperties() ?? throw new ChannelIsNullException();

            properties.ContentType = "application/json";
            properties.Persistent = true;

            await producingService.SendAsync(body, properties, "NodeEventsExchange", "unprocessed");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка отправки сообщения об ошибке обработки ивента {@Message}", message);
        }
    }
}
