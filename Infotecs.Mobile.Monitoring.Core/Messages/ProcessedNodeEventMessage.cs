namespace Infotecs.Mobile.Monitoring.Core.Messages;

/// <summary>
/// Сообщения для передачи информации об ивенте по шине сообщений.
/// </summary>
public class ProcessedNodeEventMessage
{
    /// <summary>
    /// Идентификатор ноды (устройства).
    /// </summary>
    public string NodeId { get; set; } = null!;

    /// <summary>
    /// Идентификатор ивента.
    /// </summary>
    public string EventId { get; set; } = null!;

    /// <summary>
    /// Дата регистрации ивента.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Наименование ивента.
    /// </summary>
    public string Name { get; set; } = null!;

}
