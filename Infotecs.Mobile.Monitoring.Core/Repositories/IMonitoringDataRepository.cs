using Infotecs.Mobile.Monitoring.Core.Models;

namespace Infotecs.Mobile.Monitoring.Core.Repositories;

/// <summary>
/// Интерфейс для репозитория мониторинговых данных.
/// </summary>
public interface IMonitoringDataRepository
{
    /// <summary>
    /// Создание новой записи.
    /// </summary>
    /// <param name="monitoringData">Данные мониторинга.</param>
    /// <returns>Задача.</returns>
    public Task CreateAsync(MonitoringData monitoringData);

    /// <summary>
    /// Обновление мониторинговых данных.
    /// </summary>
    /// <param name="monitoringData">Данные мониторинга.</param>
    /// <returns>Задача.</returns>
    Task UpdateAsync(MonitoringData monitoringData);

    /// <summary>
    /// Получение мониторинговых данных по идентификатору устройства.
    /// </summary>
    /// <param name="id">Идентификатор устойства.</param>
    /// <returns>Мониторинговые данные.</returns>
    public Task<MonitoringData?> GetByIdAsync(string id);

    /// <summary>
    /// Получение всех мониторинговых данных.
    /// </summary>
    /// <returns>Перечисление с мониторинговыми данными.</returns>
    public Task<IEnumerable<MonitoringData>> GetAllAsync();

    /// <summary>
    /// Поиск мониторинговых данных по набору критериев.
    /// </summary>
    /// <param name="criteria">Критерии поиска.</param>
    /// <returns>Перечисление с мониторинговыми данными.</returns>
    public Task<SearchResult<MonitoringData>> SearchAsync(MonitoringSearchCriteria criteria);

    /// <summary>
    /// Получение списка ивентов ноды.
    /// </summary>
    /// <param name="nodeId">Идентификатор ноды (устройства).</param>
    /// <returns>Список ивентов.</returns>
    Task<IEnumerable<NodeEvent>> GetEventsAsync(string nodeId);

    /// <summary>
    /// Добавление ивентов от ноды (устройства).
    /// </summary>
    /// <param name="nodeEvent">Ивент.</param>
    /// <returns>Задача.</returns>
    Task<NodeEvent> AddEventAsync(NodeEvent nodeEvent);

}
