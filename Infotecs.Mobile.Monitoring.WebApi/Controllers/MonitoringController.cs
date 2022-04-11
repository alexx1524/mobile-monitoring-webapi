using Infotecs.Mobile.Monitoring.Core.Models;
using Infotecs.Mobile.Monitoring.Core.Services;
using Infotecs.Mobile.Monitoring.WebApi.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Infotecs.Mobile.Monitoring.WebApi.Controllers;

/// <summary>
/// Контроллер для работы с данными мониторинга мобильных устройств
/// </summary>
[ApiController]
[Route("monitoring")]
public class MonitoringController : Controller
{
    private readonly ILogger<MonitoringController> logger;
    private readonly IMonitoringService monitoringService;

    public MonitoringController(ILogger<MonitoringController> logger, IMonitoringService monitoringService)
    {
        this.logger = logger;
        this.monitoringService = monitoringService;
    }

    /// <summary>
    /// Создание или обновление мониторинговых данных для устройства.
    /// </summary>
    /// <param name="request">Запрос на добавление/обновление данных мониторинга</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> AddOrUpdateAsync(AddMonitoringDataRequest request)
    {
        try
        {
            var monitoringData = request.Adapt<MonitoringData>();

            logger.LogInformation("Monitoring: {@Request}", request);

            await monitoringService.AddOrUpdateAsync(monitoringData);

            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка сохранения данных мониторинга");
            throw;
        }
    }

    /// <summary>
    /// Получение последних мониторинговых данных по идентификатору устройства
    /// </summary>
    /// <param name="id">Идентификатор устройства</param>
    /// <returns>Последние данные мониторинга или NULL, если нет данных по этому устройству</returns>
    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<MonitoringData?>> GetByIdAsync(string id)
    {
        try
        {
            MonitoringData? result = await monitoringService.GetByIdAsync(id);

            return Ok(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Ошибка получения данных по идентификатору {id}");
            throw;
        }
    }

    /// <summary>
    /// Получение всех мониторинговых данных
    /// </summary>
    /// <returns>Коллекция всех мониторинговых данных</returns>
    [HttpGet]
    [Route("list")]
    public async Task<ActionResult<IEnumerable<MonitoringData>>> GetListAsync()
    {
        try
        {
            IEnumerable<MonitoringData> result =  await monitoringService.GetListAsync();

            return Ok(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка получения списка данных мониторинга");
            throw;
        }
    }

    /// <summary>
    /// Получение мониторинговых данных по указанным критериям
    /// </summary>
    /// <returns>Коллекция всех мониторинговых данных</returns>
    [HttpPost]
    [Route("search")]
    public async Task<ActionResult<SearchResult<MonitoringData>>> SearchAsync(MonitoringSearchCriteria criteria)
    {
        try
        {
            SearchResult<MonitoringData> result = await monitoringService.SearchAsync(criteria);

            return Ok(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка поиска данных мониторинга по запросу {@Criteria}", criteria);
            throw;
        }
    }
}
