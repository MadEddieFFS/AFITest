using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AFI.Persistance.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PolicyHolder",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedByUserId = table.Column<Guid>(nullable: false),
                    LastEditedByUserId = table.Column<Guid>(nullable: true),
                    CreatedUTC = table.Column<DateTime>(nullable: false),
                    EditedUTC = table.Column<DateTime>(nullable: true),
                    PolicyId = table.Column<int>(nullable: false),
                    PolicyNumber = table.Column<string>(nullable: true),
                    Forename = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    ReferenceNumber = table.Column<string>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: true),
                    EMail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyHolder", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PolicyHolder");
        }
    }
}
