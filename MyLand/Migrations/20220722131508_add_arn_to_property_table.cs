using Microsoft.EntityFrameworkCore.Migrations;

namespace MyLand.Migrations
{
    public partial class add_arn_to_property_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "topicArn",
                table: "Property",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "topicArn",
                table: "Property");
        }
    }
}
