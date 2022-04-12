namespace Infotecs.Mobile.Monitoring.Core.Models.Sorting;

/// <summary>
/// Класс для параметров сортировки.
/// </summary>
public class SortCriteria
{
    /// <summary>
    /// Наименование поля для сортировки.
    /// </summary>
    public string? FieldName { get; set; }

    /// <summary>
    /// Направление сортировки (по возростанию или по убыванию).
    /// </summary>
    public SortOrder? Direction { get; set; }
}
