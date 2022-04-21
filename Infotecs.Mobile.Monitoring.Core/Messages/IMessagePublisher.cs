namespace Infotecs.Mobile.Monitoring.Core.Messages;

/// <summary>
/// Интерфейс для публикации сообщений в шину данных.
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Отправить сообщение об успешной обработке ивента от ноды.
    /// </summary>
    /// <param name="message">Объект сообщения об обработанном ивенте.</param>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
    Task SendProcessedNodeEventAsync(ProcessedNodeEventMessage message);

    /// <summary>
    /// Отправить сообщение об ошибке обработки ивента от ноды.
    /// </summary>
    /// <param name="message">Объект сообщения о необработанном ивенте.</param>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
    Task SendUnprocessedNodeEventAsync(UnprocessedNodeEventMessage message);
}
