namespace Infotecs.Mobile.Monitoring.WebApi.Models;

/// <summary>
/// Запрос на добавление ивента от ноды (устройства).
/// </summary>
public class AddNodeEventRequest
{
    /// <summary>
    /// Наименование ивента.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Дата/время регистрации события.
    /// </summary>
    public DateTime? Date { get; set; }
}
