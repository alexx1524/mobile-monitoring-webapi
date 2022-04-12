namespace Infotecs.Mobile.Monitoring.Data.Models;

/// <summary>
/// Класс ивента ноды для сохранения в БД.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public class NodeEventEntity
{
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
    public DateTime Date { get; set; }
}
