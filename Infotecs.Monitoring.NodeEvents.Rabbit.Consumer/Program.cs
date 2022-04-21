using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };

using (IConnection? connection = factory.CreateConnection())
using (IModel? channel = connection.CreateModel())
{
    // получение обработанных ивентов
    channel.QueueDeclare(queue: "ProcessedNodeEventQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

    var processedConcumer = new EventingBasicConsumer(channel);

    processedConcumer.Received += (sender, eventArgs) =>
    {
        string message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
        Console.WriteLine($"Зарегистрировано новое событие узла: {message}");
    };

    channel.BasicConsume(queue: "ProcessedNodeEventQueue", autoAck: true, consumer: processedConcumer);

    // получение необработанных ивентов
    channel.QueueDeclare(queue: "UnprocessedNodeEventQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

    var unprocessedConcumer = new EventingBasicConsumer(channel);

    unprocessedConcumer.Received += (sender, eventArgs) =>
    {
        string message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

        Console.WriteLine($"Ошибка регистрации события узла: {message}");
    };

    channel.BasicConsume(queue: "UnprocessedNodeEventQueue", autoAck: true, consumer: unprocessedConcumer);

    Console.WriteLine("Нажмите Enter для выхода...");
    Console.ReadLine();
}
