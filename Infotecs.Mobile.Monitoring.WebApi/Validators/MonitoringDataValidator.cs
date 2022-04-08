using FluentValidation;
using Infotecs.Mobile.Monitoring.Core.Models;

namespace Infotecs.Mobile.Monitoring.WebApi.Validators;

/// <summary>
/// Класс для валидации мониторинговых данных
/// </summary>
public class MonitoringDataValidator : AbstractValidator<MonitoringData>
{
    public MonitoringDataValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
        RuleFor(x => x.NodeName).NotNull().NotEmpty();
        RuleFor(x => x.OperatingSystem).NotNull().NotEmpty();
        RuleFor(x => x.Version).NotNull().NotEmpty();
    }
}
