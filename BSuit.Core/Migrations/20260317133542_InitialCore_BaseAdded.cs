using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSuit.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCore_BaseAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "CORE",
                table: "TenantSubscriptionModule",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 105);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "CORE",
                table: "TenantSubscriptionModule",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "CORE",
                table: "TenantSubscriptionModule",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 101);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                schema: "CORE",
                table: "TenantSubscriptionModule",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 103);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                schema: "CORE",
                table: "TenantSubscriptionModule",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 102);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                schema: "CORE",
                table: "TenantSubscriptionModule",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 104);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "CORE",
                table: "TenantSubscription",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 105);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "CORE",
                table: "TenantSubscription",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "CORE",
                table: "TenantSubscription",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 101);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                schema: "CORE",
                table: "TenantSubscription",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 103);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                schema: "CORE",
                table: "TenantSubscription",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 102);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                schema: "CORE",
                table: "TenantSubscription",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 104);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "CORE",
                table: "TenantMaster",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 105);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                schema: "CORE",
                table: "TenantMaster",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("Relational:ColumnOrder", 103);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "CORE",
                table: "TenantMaster",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "CORE",
                table: "TenantMaster",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 101);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                schema: "CORE",
                table: "TenantMaster",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 102);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                schema: "CORE",
                table: "TenantMaster",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 104);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "CORE",
                table: "SubscriptionMaster",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 105);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "CORE",
                table: "SubscriptionMaster",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 1)
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "CORE",
                table: "SubscriptionMaster",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 101);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                schema: "CORE",
                table: "SubscriptionMaster",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 103);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                schema: "CORE",
                table: "SubscriptionMaster",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 102);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                schema: "CORE",
                table: "SubscriptionMaster",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 104);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "CORE",
                table: "ModuleMaster",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 105);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "CORE",
                table: "ModuleMaster",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 1)
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "CORE",
                table: "ModuleMaster",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 101);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                schema: "CORE",
                table: "ModuleMaster",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 103);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                schema: "CORE",
                table: "ModuleMaster",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 102);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                schema: "CORE",
                table: "ModuleMaster",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 104);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "CORE",
                table: "Logs",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Relational:ColumnOrder", 1)
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "CORE",
                table: "AuditLog",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Relational:ColumnOrder", 1)
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "CORE",
                table: "TenantSubscriptionModule");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                schema: "CORE",
                table: "TenantSubscriptionModule");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "CORE",
                table: "TenantSubscriptionModule");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                schema: "CORE",
                table: "TenantSubscriptionModule");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "CORE",
                table: "TenantSubscription");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                schema: "CORE",
                table: "TenantSubscription");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "CORE",
                table: "TenantSubscription");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                schema: "CORE",
                table: "TenantSubscription");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "CORE",
                table: "TenantMaster");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "CORE",
                table: "TenantMaster");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                schema: "CORE",
                table: "TenantMaster");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "CORE",
                table: "SubscriptionMaster");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                schema: "CORE",
                table: "SubscriptionMaster");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "CORE",
                table: "SubscriptionMaster");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                schema: "CORE",
                table: "SubscriptionMaster");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "CORE",
                table: "ModuleMaster");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                schema: "CORE",
                table: "ModuleMaster");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "CORE",
                table: "ModuleMaster");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                schema: "CORE",
                table: "ModuleMaster");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "CORE",
                table: "TenantSubscriptionModule",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 105);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "CORE",
                table: "TenantSubscriptionModule",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "CORE",
                table: "TenantSubscription",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 105);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "CORE",
                table: "TenantSubscription",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "CORE",
                table: "TenantMaster",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 105);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                schema: "CORE",
                table: "TenantMaster",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .OldAnnotation("Relational:ColumnOrder", 103);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "CORE",
                table: "TenantMaster",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "CORE",
                table: "SubscriptionMaster",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 105);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "CORE",
                table: "SubscriptionMaster",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("Relational:ColumnOrder", 1)
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "CORE",
                table: "ModuleMaster",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 105);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "CORE",
                table: "ModuleMaster",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("Relational:ColumnOrder", 1)
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "CORE",
                table: "Logs",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("Relational:ColumnOrder", 1)
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "CORE",
                table: "AuditLog",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("Relational:ColumnOrder", 1)
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}
