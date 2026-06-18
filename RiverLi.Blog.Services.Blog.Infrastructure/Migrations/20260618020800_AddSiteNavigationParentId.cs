using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverLi.Blog.Services.Blog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteNavigationParentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Blog_SiteNavigations",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Blog_SiteNavigations");
        }
    }
}
