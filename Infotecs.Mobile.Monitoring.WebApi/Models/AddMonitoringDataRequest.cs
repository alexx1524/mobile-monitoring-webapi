using System.Runtime.InteropServices;
using Infotecs.Mobile.Monitoring.Core.Models;

namespace Infotecs.Mobile.Monitoring.WebApi.Models;

/// <summary>
/// Запрос на добавление данных мониторинга.
/// </summary>
public class AddMonitoringDataRequest
{
    /// <summary>
    /// Идентификатор устройства.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Имя узла / пользователя.
    /// </summary>
    public string? NodeName { get; set; }

    /// <summary>
    /// Наименование операционной системы.
    /// </summary>
    public string? OperatingSystem { get; set; }

    /// <summary>
    /// Версия клиента.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Список ивентов от ноды.
    /// </summary>
    public IEnumerable<NodeEvent> Events { get; set; } = Array.Empty<NodeEvent>();

}
