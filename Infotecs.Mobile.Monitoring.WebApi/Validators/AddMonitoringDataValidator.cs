using FluentValidation;
using Infotecs.Mobile.Monitoring.Core.Models;

namespace Infotecs.Mobile.Monitoring.WebApi.Validators;

/// <summary>
/// Класс для валидации запроса на создание/изменение мониторинговых данных.
/// </summary>
public class AddMonitoringDataValidator : AbstractValidator<MonitoringData>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public AddMonitoringDataValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
        RuleFor(x => x.NodeName).NotNull().NotEmpty();
        RuleFor(x => x.OperatingSystem).NotNull().NotEmpty();
        RuleFor(x => x.Version).NotNull().NotEmpty();
    }
}
