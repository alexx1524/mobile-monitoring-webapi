using Infotecs.Mobile.Monitoring.Core.Messages;
using Infotecs.Mobile.Monitoring.Core.Models;
using Infotecs.Mobile.Monitoring.Core.Repositories;
using Infotecs.Mobile.Monitoring.Core.Services;
using Infotecs.Mobile.Monitoring.WebApi.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Infotecs.Mobile.Monitoring.WebApi.Controllers;

/// <summary>
/// Контроллер для работы с ивентами от нод (устройств).
/// </summary>
[ApiController]
[Route("event")]
public class NodeEventController : Controller
{

    private readonly IMonitoringService monitoringService;
    private readonly ILogger<NodeEventController> logger;
    private readonly IUnitOfWork unitOfWork;
    private readonly IMessagePublisher messagePublisher;

    /// <summary>
    /// Конструктор класса.
    /// </summary>
    /// <param name="monitoringService">Интерфейс сервиса мониторинга.</param>
    /// <param name="logger">Интерфейс логгирования.</param>
    /// <param name="unitOfWork">Интерфейс управления транзакицями.</param>
    /// <param name="messagePublisher">Интерфейс для публикации сообщений о статусе обработки ивентов.</param>
    public NodeEventController(IMonitoringService monitoringService,
        ILogger<NodeEventController> logger,
        IUnitOfWork unitOfWork,
        IMessagePublisher messagePublisher)
    {
        this.monitoringService = monitoringService;
        this.logger = logger;
        this.unitOfWork = unitOfWork;
        this.messagePublisher = messagePublisher;
    }

    /// <summary>
    /// Получение списка событий по идентификатору ноды (устройства).
    /// </summary>
    /// <param name="nodeId">Идентификатор узла (устройства).</param>
    /// <returns>Список ивентов.</returns>
    [HttpGet]
    [Route("bynode/{nodeId}")]
    public async Task<ActionResult<IEnumerable<NodeEvent>>> GetEventsByNodeIdAsync(string nodeId)
    {
        IEnumerable<NodeEvent> events = await monitoringService.GetNodeEventsAsync(nodeId);

        return Ok(events);
    }

}
