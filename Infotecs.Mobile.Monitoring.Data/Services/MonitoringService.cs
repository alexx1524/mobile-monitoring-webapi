using Infotecs.Mobile.Monitoring.Core.Models;
using Infotecs.Mobile.Monitoring.Core.Repositories;
using Infotecs.Mobile.Monitoring.Core.Services;

namespace Infotecs.Mobile.Monitoring.Data.Services;

/// <summary>
/// Сервис работы с мониторинговыми данными.
/// </summary>
public class MonitoringService : IMonitoringService
{
    private readonly IMonitoringDataRepository monitoringDataRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="monitoringDataRepository">Репозиторий для работы с мониторингвыми данными.</param>
    public MonitoringService(IMonitoringDataRepository monitoringDataRepository) =>
        this.monitoringDataRepository = monitoringDataRepository;

    /// <summary>
    /// Создание или обновление данных мониторинга.
    /// </summary>
    /// <param name="monitoringData">Данные мониторинга.</param>
    /// <exception cref="ArgumentNullException">Исключение вызывается, если мониторингавые данные или идентификатор устроства равны Null или пустой строке.</exception>
    /// <returns>Задача.</returns>
    public async Task AddOrUpdateAsync(MonitoringData monitoringData)
    {
        if (string.IsNullOrEmpty(monitoringData?.Id))
        {
            throw new ArgumentNullException(nameof(monitoringData));
        }

        MonitoringData? existedMonitoringData = await monitoringDataRepository.GetById(monitoringData.Id);

        if (existedMonitoringData is null)
        {
            monitoringData.CreatedDate =
                monitoringData.UpdatedDate =
                    DateTime.UtcNow;

            await monitoringDataRepository.Create(monitoringData);
        }
        else
        {
            existedMonitoringData.NodeName = monitoringData.NodeName;
            existedMonitoringData.OperatingSystem = monitoringData.OperatingSystem;
            existedMonitoringData.Version = monitoringData.Version;
            existedMonitoringData.UpdatedDate = DateTime.UtcNow;

            await monitoringDataRepository.Update(existedMonitoringData);
        }
    }

    /// <summary>
    /// Получение данных мониторинга по идентификатору устройства.
    /// </summary>
    /// <param name="id">Идентификатор устройства.</param>
    /// <returns>Мониторинговые данные по указанному идентификатору.</returns>
    public async Task<MonitoringData?> GetByIdAsync(string id)
    {
        MonitoringData? result = await monitoringDataRepository.GetById(id);

        return result;
    }

    /// <summary>
    /// Получение всех данных мониторинга.
    /// </summary>
    /// <returns>Все мониторинговые данные.</returns>
    public async Task<IEnumerable<MonitoringData>> GetListAsync() =>
        await monitoringDataRepository.GetAll();

    /// <summary>
    /// Поиск мониторинговых данных по набору критериев.
    /// </summary>
    /// <param name="criteria">Набор критериев поиска.</param>
    /// <returns>Результаты поиска мониторинговых данных.</returns>
    public async Task<SearchResult<MonitoringData>> SearchAsync(MonitoringSearchCriteria criteria)
    {
        SearchResult<MonitoringData> result = await monitoringDataRepository.Search(criteria);

        return result;
    }

    /// <summary>
    /// Получение списка ивентов для указанной ноды (устройства).
    /// </summary>
    /// <param name="nodeId">Идентификатор ноды (устройства).</param>
    /// <returns>Список ивентов.</returns>
    public Task<IEnumerable<NodeEvent>> GetNodeEvents(string nodeId) => monitoringDataRepository.GetEvents(nodeId);

    /// <summary>
    /// Добавление ивентов от ноды (устройства).
    /// </summary>
    /// <param name="nodeEvent">Ивент от ноды (устройства).</param>
    /// <returns>Задача.</returns>
    public Task AddEvent(NodeEvent nodeEvent) => monitoringDataRepository.AddEvent(nodeEvent);

}
