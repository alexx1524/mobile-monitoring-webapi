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
        Create.Table("monitoring_data")
            .WithColumn("id").AsString(128).NotNullable().PrimaryKey()
            .WithColumn("nodename").AsString(256).NotNullable()
            .WithColumn("operatingsystem").AsString(64).NotNullable()
            .WithColumn("version").AsString(64).NotNullable()
            .WithColumn("createddate").AsDateTimeOffset().NotNullable()
            .WithColumn("updateddate").AsDateTimeOffset().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("monitoring_data");
    }
}
