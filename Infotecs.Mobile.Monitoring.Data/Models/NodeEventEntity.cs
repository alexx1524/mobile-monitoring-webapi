namespace Infotecs.Mobile.Monitoring.Data.Models;

/// <summary>
/// Класс ивента ноды для сохранения в БД.
/// </summary>
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
    /// Описание события.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Дата/время регистрации события.
    /// </summary>
    public DateTime Date { get; set; }
}
