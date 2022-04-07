using Microsoft.AspNetCore.Mvc;

namespace Infotecs.Mobile.Monitoring.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MonitoringController : Controller
{
    private readonly ILogger<MonitoringController> logger;

    public MonitoringController(ILogger<MonitoringController> logger)
    {
        this.logger = logger;
    }
}
