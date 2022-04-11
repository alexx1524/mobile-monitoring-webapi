namespace Infotecs.Mobile.Monitoring.Core.Models;

/// <summary>
/// Класс для возврата рзультатов поиска
/// </summary>
/// <typeparam name="T"></typeparam>
public class SearchResult<T>
{
    /// <summary>
    /// Общее количество элементов
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Список элементов
    /// </summary>
    public IEnumerable<T> Items { get; set; }
}
