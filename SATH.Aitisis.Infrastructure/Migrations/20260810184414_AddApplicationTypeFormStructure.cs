using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SATH.Aitisis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationTypeFormStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentRequirementId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationTypeFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ApplicationTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Label = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FieldType = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationTypeFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationTypeFields_ApplicationTypes_ApplicationTypeId",
                        column: x => x.ApplicationTypeId,
                        principalTable: "ApplicationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DocumentRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ApplicationTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRequired = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentRequirements_ApplicationTypes_ApplicationTypeId",
                        column: x => x.ApplicationTypeId,
                        principalTable: "ApplicationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApplicationFieldValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    ApplicationTypeFieldId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationFieldValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationFieldValues_ApplicationTypeFields_ApplicationType~",
                        column: x => x.ApplicationTypeFieldId,
                        principalTable: "ApplicationTypeFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationFieldValues_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentRequirementId",
                table: "Documents",
                column: "DocumentRequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationFieldValues_ApplicationId_ApplicationTypeFieldId",
                table: "ApplicationFieldValues",
                columns: new[] { "ApplicationId", "ApplicationTypeFieldId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationFieldValues_ApplicationTypeFieldId",
                table: "ApplicationFieldValues",
                column: "ApplicationTypeFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationTypeFields_ApplicationTypeId_Name",
                table: "ApplicationTypeFields",
                columns: new[] { "ApplicationTypeId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRequirements_ApplicationTypeId",
                table: "DocumentRequirements",
                column: "ApplicationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_DocumentRequirements_DocumentRequirementId",
                table: "Documents",
                column: "DocumentRequirementId",
                principalTable: "DocumentRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DocumentRequirements_DocumentRequirementId",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "ApplicationFieldValues");

            migrationBuilder.DropTable(
                name: "DocumentRequirements");

            migrationBuilder.DropTable(
                name: "ApplicationTypeFields");

            migrationBuilder.DropIndex(
                name: "IX_Documents_DocumentRequirementId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DocumentRequirementId",
                table: "Documents");
        }
    }
}
