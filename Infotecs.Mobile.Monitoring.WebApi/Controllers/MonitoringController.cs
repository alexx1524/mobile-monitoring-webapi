using Infotecs.Mobile.Monitoring.Core.ClientsInforming;
using Infotecs.Mobile.Monitoring.Core.Messages;
using Infotecs.Mobile.Monitoring.Core.Models;
using Infotecs.Mobile.Monitoring.Core.Repositories;
using Infotecs.Mobile.Monitoring.Core.Services;
using Infotecs.Mobile.Monitoring.WebApi.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Infotecs.Mobile.Monitoring.WebApi.Controllers;

/// <summary>
/// Контроллер для работы с данными мониторинга мобильных устройств.
/// </summary>
[ApiController]
[Route("monitoring")]
public class MonitoringController : Controller
{

    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<MonitoringController> logger;
    private readonly IMonitoringService monitoringService;
    private readonly IChangeNotifier changeNotifier;
    private readonly IMessagePublisher messagePublisher;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="dbContext">Контекст для работы с базой данных.</param>
    /// <param name="logger">Интерфейс логгирования.</param>
    /// <param name="monitoringService">Интерфейс сервиса мониторинговых данных.</param>
    /// <param name="changeNotifier">Интерфейс для информирования об изменении данных на сервере.</param>
    /// <param name="messagePublisher">Интерфейс для отправкии сообщений о статусе обработки ивентов.</param>
    public MonitoringController(IUnitOfWork dbContext,
        ILogger<MonitoringController> logger,
        IMonitoringService monitoringService,
        IChangeNotifier changeNotifier,
        IMessagePublisher messagePublisher)
    {
        this.unitOfWork = dbContext;
        this.logger = logger;
        this.monitoringService = monitoringService;
        this.changeNotifier = changeNotifier;
        this.messagePublisher = messagePublisher;
    }

    /// <summary>
    /// Создание или обновление мониторинговых данных для устройства.
    /// </summary>
    /// <param name="request">Запрос на добавление/обновление данных мониторинга.</param>
    /// <returns>Результат ActionResult.</returns>
    [HttpPost]
    public async Task<ActionResult> AddOrUpdateAsync(AddMonitoringDataRequest request)
    {
        var monitoringData = request.Adapt<MonitoringData>();

        try
        {
            logger.LogInformation("Monitoring: {@Request}", request);

            bool isCreated = await monitoringService.AddOrUpdateAsync(monitoringData);

            unitOfWork.Commit();

            if (isCreated)
            {
                await changeNotifier.SendNewMonitoringDataAsync(monitoringData);
            }
        }
        catch (Exception e)
        {
            unitOfWork.Rollback();

            logger.LogError(e, "Ошибка сохранения данных мониторинга");

            throw;
        }

        foreach (AddNodeEventRequest nodeEventRequest in request.Events)
        {
            try
            {
                logger.LogInformation("Ивент {@NodeEvent}", nodeEventRequest);

                var nodeEvent = (nodeEventRequest, request.Id).Adapt<NodeEvent>(TypeAdapterConfig<(AddNodeEventRequest, string), NodeEvent>
                    .NewConfig()
                    .Map(dest => dest.NodeId, src => src.Item2)
                    .Map(dest => dest.Name, src => src.Item1.Name)
                    .Map(dest => dest.Date, src => src.Item1.Date)
                    .Config);

                nodeEvent = await monitoringService.AddEventAsync(nodeEvent);

                unitOfWork.Commit();

                await messagePublisher.SendProcessedNodeEventAsync(nodeEvent.Adapt<ProcessedNodeEventMessage>());
            }
            catch (Exception e)
            {
                unitOfWork.Rollback();

                var message = (nodeEventRequest, request.Id, e.Message)
                    .Adapt<UnprocessedNodeEventMessage>(TypeAdapterConfig<(AddNodeEventRequest, string, string), UnprocessedNodeEventMessage>
                        .NewConfig()
                        .Map(dest => dest.NodeId, src => src.Item2)
                        .Map(dest => dest.Name, src => src.Item1.Name)
                        .Map(dest => dest.Date, src => src.Item1.Date)
                        .Map(dest => dest.ErrorMessage, src => src.Item3)
                        .Config);

                await messagePublisher.SendUnprocessedNodeEventAsync(message);

                logger.LogError(e, "Ошибка сохранения ивента {@NodeEvent}", nodeEventRequest);
            }
        }

        return Ok();
    }

    /// <summary>
    /// Получение последних мониторинговых данных по идентификатору устройства.
    /// </summary>
    /// <param name="id">Идентификатор устройства.</param>
    /// <returns>Последние данные мониторинга или NULL, если нет данных по этому устройству.</returns>
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
            logger.LogError(e, "Ошибка получения данных по идентификатору {@Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Получение всех мониторинговых данных.
    /// </summary>
    /// <returns>Коллекция всех мониторинговых данных.</returns>
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
    /// Получение мониторинговых данных по указанным критериям.
    /// </summary>
    /// <param name="criteria">Критерии поиска.</param>
    /// <returns>Коллекция всех мониторинговых данных.</returns>
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
