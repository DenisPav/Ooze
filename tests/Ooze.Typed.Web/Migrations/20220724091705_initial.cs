using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ooze.Typed.Web.Migrations;

public partial class initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Blog",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Blog", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Post",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                Body = table.Column<string>(type: "TEXT", nullable: false),
                BlogId = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Post", x => x.Id);
                table.ForeignKey(
                    name: "FK_Post_Blog_BlogId",
                    column: x => x.BlogId,
                    principalTable: "Blog",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Comment",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Text = table.Column<string>(type: "TEXT", nullable: false),
                UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                PostId = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Comment", x => x.Id);
                table.ForeignKey(
                    name: "FK_Comment_Post_PostId",
                    column: x => x.PostId,
                    principalTable: "Post",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Comment_PostId",
            table: "Comment",
            column: "PostId");

        migrationBuilder.CreateIndex(
            name: "IX_Post_BlogId",
            table: "Post",
            column: "BlogId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Comment");

        migrationBuilder.DropTable(
            name: "Post");

        migrationBuilder.DropTable(
            name: "Blog");
    }
}
