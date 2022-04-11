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
    /// <param name="monitoringData">Данные мониторинга</param>
    /// <returns>Задача</returns>
    Task AddOrUpdateAsync(MonitoringData monitoringData);

    /// <summary>
    /// Получение данных мониторинга по идентификатору устройства
    /// </summary>
    /// <param name="id">Идентификатор устройства</param>
    /// <returns>Задача, возвращающая данные мониторинга</returns>
    Task<MonitoringData?> GetByIdAsync(string id);

    /// <summary>
    /// Получение всех данных мониторинга
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<MonitoringData>> GetListAsync();

    /// <summary>
    /// Поиск мониторинговых данных по набору критериев с сортировкой и пагинацией
    /// </summary>
    /// <returns></returns>
    Task<SearchResult<MonitoringData>> SearchAsync(MonitoringSearchCriteria criteria);

}
