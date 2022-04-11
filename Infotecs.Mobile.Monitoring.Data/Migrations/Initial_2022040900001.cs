using FluentMigrator;

namespace Infotecs.Mobile.Monitoring.Data.Migrations;

/// <summary>
/// Создание таблицы для мониторинговых данных
/// </summary>
[Migration(2022040900001)]
public class Initial_2022040900001 : Migration
{
    public override void Up()
    {
        Create.Table("MonitoringData")
            .WithColumn("Id").AsString(128).NotNullable().PrimaryKey()
            .WithColumn("NodeName").AsString(256).NotNullable()
            .WithColumn("OperatingSystem").AsString(64).NotNullable()
            .WithColumn("Version").AsString(64).NotNullable()
            .WithColumn("CreatedDate").AsDateTimeOffset().NotNullable()
            .WithColumn("UpdatedDate").AsDateTimeOffset().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("MonitoringData");
    }
}
