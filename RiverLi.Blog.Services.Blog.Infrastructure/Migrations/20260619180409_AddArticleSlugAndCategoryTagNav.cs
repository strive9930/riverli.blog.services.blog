using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverLi.Blog.Services.Blog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleSlugAndCategoryTagNav : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.AddColumn<string>(
            //     name: "Slug",
            //     table: "Blog_Articles",
            //     type: "varchar(250)",
            //     maxLength: 250,
            //     nullable: false,
            //     defaultValue: "")
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Blog_ArticleTags_TagId",
            //     table: "Blog_ArticleTags",
            //     column: "TagId");
            //
            // migrationBuilder.CreateIndex(
            //     name: "IX_Blog_Articles_CategoryId",
            //     table: "Blog_Articles",
            //     column: "CategoryId");
            //
            // migrationBuilder.CreateIndex(
            //     name: "IX_Blog_Articles_Slug",
            //     table: "Blog_Articles",
            //     column: "Slug",
            //     unique: true);
            //
            // migrationBuilder.AddForeignKey(
            //     name: "FK_Blog_Articles_Blog_Categories_CategoryId",
            //     table: "Blog_Articles",
            //     column: "CategoryId",
            //     principalTable: "Blog_Categories",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Restrict);
            //
            // migrationBuilder.AddForeignKey(
            //     name: "FK_Blog_ArticleTags_Blog_Tags_TagId",
            //     table: "Blog_ArticleTags",
            //     column: "TagId",
            //     principalTable: "Blog_Tags",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Blog_Articles_Blog_Categories_CategoryId",
            //     table: "Blog_Articles");
            //
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Blog_ArticleTags_Blog_Tags_TagId",
            //     table: "Blog_ArticleTags");
            //
            // migrationBuilder.DropIndex(
            //     name: "IX_Blog_ArticleTags_TagId",
            //     table: "Blog_ArticleTags");
            //
            // migrationBuilder.DropIndex(
            //     name: "IX_Blog_Articles_CategoryId",
            //     table: "Blog_Articles");
            //
            // migrationBuilder.DropIndex(
            //     name: "IX_Blog_Articles_Slug",
            //     table: "Blog_Articles");
            //
            // migrationBuilder.DropColumn(
            //     name: "Slug",
            //     table: "Blog_Articles");
        }
    }
}
