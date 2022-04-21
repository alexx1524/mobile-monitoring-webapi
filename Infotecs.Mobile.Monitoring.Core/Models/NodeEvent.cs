namespace Infotecs.Mobile.Monitoring.Core.Models;

/// <summary>
/// Событие от ноды (устройства).
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public class NodeEvent
{
    /// <summary>
    /// Идентификатор ивента.
    /// </summary>
    public string EventId { get; set; } = null!;

    /// <summary>
    /// Идентификатор ноды.
    /// </summary>
    public string NodeId { get; set; } = null!;

    /// <summary>
    /// Наименование ивента.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Дата/время регистрации события.
    /// </summary>
    public DateTime? Date { get; set; }
}
