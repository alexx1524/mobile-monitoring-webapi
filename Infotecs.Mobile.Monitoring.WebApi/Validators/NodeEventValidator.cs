using FluentValidation;
using Infotecs.Mobile.Monitoring.Core.Models;

namespace Infotecs.Mobile.Monitoring.WebApi.Validators;

/// <summary>
/// Класс валидации ивента от ноды.
/// </summary>
public class NodeEventValidator : AbstractValidator<NodeEvent>
{

    /// <summary>
    /// Конструктор.
    /// </summary>
    public NodeEventValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).NotNull().NotEmpty().MaximumLength(256);
        RuleFor(x => x.Date).NotNull();
        RuleFor(x => x.NodeId).NotNull().NotEmpty();
    }
}
