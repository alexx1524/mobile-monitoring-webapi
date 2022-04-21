namespace Infotecs.Mobile.Monitoring.Core.Messages;

/// <summary>
/// Сообщение о необработанном ивенте от ноды (устройства).
/// </summary>
public class UnprocessedNodeEventMessage
{
    /// <summary>
    /// Идентификатор ноды (устройства).
    /// </summary>
    public string NodeId { get; set; } = null!;

    /// <summary>
    /// Дата регистрации ивента.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Наименование ивента.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    public string ErrorMessage { get; set; } = null!;
}
