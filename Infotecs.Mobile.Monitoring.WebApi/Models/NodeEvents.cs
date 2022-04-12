using Infotecs.Mobile.Monitoring.Core.Models;

namespace Infotecs.Mobile.Monitoring.WebApi.Models;

/// <summary>
/// Класс для возврата ивентов ноды (устройства).
/// </summary>
public class NodeEvents
{

    /// <summary>
    /// Идентификатор ноды (устройства).
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Список ивентов.
    /// </summary>
    public IEnumerable<NodeEvent> Events { get; set; } = null!;

}
