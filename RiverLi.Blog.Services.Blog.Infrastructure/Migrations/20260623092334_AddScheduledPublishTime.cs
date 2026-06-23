using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverLi.Blog.Services.Blog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduledPublishTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledPublishTime",
                table: "Blog_Articles",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledPublishTime",
                table: "Blog_Articles");
        }
    }
}
