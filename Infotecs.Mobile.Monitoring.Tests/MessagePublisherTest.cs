using System;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Infotecs.Mobile.Monitoring.Core.Messages;
using Infotecs.Mobile.Monitoring.Data.Messages;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using Xunit;

namespace Infotecs.Mobile.Monitoring.Tests;

/// <summary>
/// Тест для проверки валидности моделей, которые передаются через RabbitMQ.
/// </summary>
public class MessagePublisherTest
{

    private readonly Mock<ILogger<RabbitMessagePublisher>> loggerMock;
    private readonly Mock<IProducingService> producingServiceMock;
    private readonly Mock<IModel> channelMock;
    private readonly Mock<IBasicProperties> basicProperties;
    private readonly RabbitMessagePublisher messagePublisher;
    private ReadOnlyMemory<byte> body;

    /// <summary>
    /// Конструктор теста.
    /// </summary>
    public MessagePublisherTest()
    {
        loggerMock = new Mock<ILogger<RabbitMessagePublisher>>();
        producingServiceMock = new Mock<IProducingService>();
        channelMock = new Mock<IModel>();
        basicProperties = new Mock<IBasicProperties>();

        channelMock.Setup(x => x.CreateBasicProperties()).Returns(() => basicProperties.Object);

        producingServiceMock.SetupGet(x => x.Channel).Returns(channelMock.Object);

        producingServiceMock
            .Setup(x => x.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<IBasicProperties>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback(((ReadOnlyMemory<byte> b, IBasicProperties p, string exchangName, string routinmgKey) => { body = b; }));

        messagePublisher = new RabbitMessagePublisher(loggerMock.Object, producingServiceMock.Object);
    }

    /// <summary>
    /// Метод проверки camelCase свойств и структуры сообщения.
    /// </summary>
    /// <param name="message">Сообщение для отправки в RabbitMq.</param>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous unit test.</placeholder></returns>
    [Theory, AutoData]
    public async Task SendProcessedNodeEventAsync_ValidSerializedMessage(ProcessedNodeEventMessage message)
    {
        // Arrange
        string jsonData = "{\"nodeId\":\"" + message.NodeId +
            "\",\"eventId\":\""+ message.EventId +
            "\",\"date\":\""+ message.Date.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFK") +
            "\",\"name\":\"" + message.Name +"\"}";

        // Act
        await messagePublisher.SendProcessedNodeEventAsync(message);

        string result = Encoding.UTF8.GetString(body.ToArray());

        // Assert
        result.Should()
            .NotBeNull()
            .And.BeEquivalentTo(jsonData);
    }

    /// <summary>
    /// Метод проверки camelCase свойств и структуры сообщения.
    /// </summary>
    /// <param name="message">Сообщение для отправки в RabbitMq.</param>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous unit test.</placeholder></returns>
    [Theory, AutoData]
    public async Task SendUnprocessedNodeEventAsync_ValidSerializedMessage(UnprocessedNodeEventMessage message)
    {
        // Arrange
        string jsonData = "{\"nodeId\":\"" + message.NodeId +
            "\",\"date\":\""+ message.Date.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFK") +
            "\",\"name\":\"" + message.Name +
            "\",\"errorMessage\":\"" + message.ErrorMessage +
            "\"}";

        // Act
        await messagePublisher.SendUnprocessedNodeEventAsync(message);

        string result = Encoding.UTF8.GetString(body.ToArray());

        // Assert
        result.Should()
            .NotBeNull()
            .And.BeEquivalentTo(jsonData);
    }
}
