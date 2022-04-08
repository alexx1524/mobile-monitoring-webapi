using FluentValidation;
using Infotecs.Mobile.Monitoring.Core.Models;

namespace Infotecs.Mobile.Monitoring.WebApi.Validators;

public class AddMonitoringDataValidator : AbstractValidator<MonitoringData>
{
    public AddMonitoringDataValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
        RuleFor(x => x.NodeName).NotNull().NotEmpty();
        RuleFor(x => x.OperatingSystem).NotNull().NotEmpty();
        RuleFor(x => x.Version).NotNull().NotEmpty();
    }
}
