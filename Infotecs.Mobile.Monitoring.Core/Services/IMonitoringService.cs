using Infotecs.Mobile.Monitoring.Core.Models;

namespace Infotecs.Mobile.Monitoring.Core.Services;

/// <summary>
/// Интерфейс сервиса для работы с мониторинговыми данными от устройств.
/// </summary>
public interface IMonitoringService
{
    /// <summary>
    /// Создание новой записи или обновление существующей записи.
    /// </summary>
    /// <param name="monitoringData">Данные мониторинга.</param>
    /// <returns>Задача.</returns>
    Task AddOrUpdateAsync(MonitoringData monitoringData);

    /// <summary>
    /// Получение данных мониторинга по идентификатору устройства.
    /// </summary>
    /// <param name="id">Идентификатор устройства.</param>
    /// <returns>Задача, возвращающая данные мониторинга.</returns>
    Task<MonitoringData?> GetByIdAsync(string id);

    /// <summary>
    /// Получение всех данных мониторинга.
    /// </summary>
    /// <returns>Список мониторинговых данных.</returns>
    Task<IEnumerable<MonitoringData>> GetListAsync();

    /// <summary>
    /// Поиск мониторинговых данных по набору критериев с сортировкой и пагинацией.
    /// </summary>
    /// <param name="criteria">Критерии поиска.</param>
    /// <returns>Результаты поиска: количество элементов и список элементов.</returns>
    Task<SearchResult<MonitoringData>> SearchAsync(MonitoringSearchCriteria criteria);

    /// <summary>
    /// Получение всех ивентов по идентификатору ноды (устройства).
    /// </summary>
    /// <param name="nodeId">Идентификатор ноды (устройства).</param>
    /// <returns>Список ивенонтов для ноды (устройства).</returns>
    Task<IEnumerable<NodeEvent>> GetNodeEvents(string nodeId);

    /// <summary>
    /// Добавление ивентов от ноды.
    /// </summary>
    /// <param name="nodeId">Идентификатор ноды.</param>
    /// <param name="events">Список ивентов.</param>
    /// <returns>Задача.</returns>
    Task AddEvents(string nodeId, IEnumerable<NodeEvent> events);

}
