using Infotecs.Mobile.Monitoring.Core.Models.Sorting;

namespace Infotecs.Mobile.Monitoring.Core.Models;

/// <summary>
/// Класс для поиска мониторинговых данных
/// </summary>
public class MonitoringSearchCriteria
{
    /// <summary>
    /// Сортировать по этому полю
    /// </summary>
    public SortCriteria? Sorting { get; set; }

    /// <summary>
    /// Номер страницы
    /// </summary>
    public int? PageNumber { get; set; }

    /// <summary>
    /// Количество элементов на странице
    /// </summary>
    public int? PageSize { get; set; }
}
