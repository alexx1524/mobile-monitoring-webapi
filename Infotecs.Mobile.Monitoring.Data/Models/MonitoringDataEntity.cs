using System.ComponentModel.DataAnnotations;

namespace Infotecs.Mobile.Monitoring.Data.Models;

/// <summary>
/// Класс данных монитолинга для сохранения в БД
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public class MonitoringDataEntity
{

    /// <summary>
    /// Идентификатор устройства.
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Имя узла / пользователя
    /// </summary>
    public string NodeName { get; set; } = null!;

    /// <summary>
    /// Наименование операционной системы
    /// </summary>
    public string OperatingSystem { get; set; } = null!;

    /// <summary>
    /// Версия клиента
    /// </summary>
    public string? Version { get; set; } = null!;

    /// <summary>
    /// Дата/время создания мониторинговых данных
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Дата/время последнего обновления мониторинговых данных
    /// </summary>
    public DateTime? UpdatedDate { get; set; }
}
