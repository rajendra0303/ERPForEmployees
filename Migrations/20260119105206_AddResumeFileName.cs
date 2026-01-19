using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPForEmployees.Migrations
{
    /// <inheritdoc />
    public partial class AddResumeFileName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResumeFileName",
                table: "JobApplications",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResumeFileName",
                table: "JobApplications");
        }
    }
}
