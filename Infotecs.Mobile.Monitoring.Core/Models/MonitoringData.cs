namespace Infotecs.Mobile.Monitoring.Core.Models;

/// <summary>
/// Данные мониторинга от мобильных приложений.
/// </summary>
public class MonitoringData
{
    /// <summary>
    /// Идентификатор устройства.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Имя узла / пользователя
    /// </summary>
    public string? NodeName { get; set; }

    /// <summary>
    /// Наименование операционной системы
    /// </summary>
    public string? OperatingSystem { get; set; }

    /// <summary>
    /// Версия клиента
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Дата/время создания мониторинговых данных
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Дата/время последнего обновления мониторинговых данных
    /// </summary>
    public DateTime? UpdatedDate { get; set; }
}
