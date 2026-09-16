using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrugPrioritizationAssistant.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Drugs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GenericName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActiveIngredient = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DosageForm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TherapeuticCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drugs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DrugNeedAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DrugId = table.Column<int>(type: "int", nullable: false),
                    ShortageSeverity = table.Column<int>(type: "int", nullable: false),
                    ImportDependency = table.Column<int>(type: "int", nullable: false),
                    DomesticProductionLevel = table.Column<int>(type: "int", nullable: false),
                    DemandLevel = table.Column<int>(type: "int", nullable: false),
                    TherapeuticImportance = table.Column<int>(type: "int", nullable: false),
                    SupplyInstability = table.Column<int>(type: "int", nullable: false),
                    NeedScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrugNeedAssessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrugNeedAssessments_Drugs_DrugId",
                        column: x => x.DrugId,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DrugScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DrugId = table.Column<int>(type: "int", nullable: false),
                    NeedScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    FeasibilityScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    OpportunityScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrugScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrugScores_Drugs_DrugId",
                        column: x => x.DrugId,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionFeasibilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DrugId = table.Column<int>(type: "int", nullable: false),
                    RawMaterialAvailability = table.Column<int>(type: "int", nullable: false),
                    RawMaterialCost = table.Column<int>(type: "int", nullable: false),
                    SynthesisComplexity = table.Column<int>(type: "int", nullable: false),
                    EnzymaticRoutePotential = table.Column<int>(type: "int", nullable: false),
                    ExpectedYield = table.Column<int>(type: "int", nullable: false),
                    PurityPotential = table.Column<int>(type: "int", nullable: false),
                    ScaleUpFeasibility = table.Column<int>(type: "int", nullable: false),
                    EquipmentAvailability = table.Column<int>(type: "int", nullable: false),
                    TechnicalRisk = table.Column<int>(type: "int", nullable: false),
                    FeasibilityScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionFeasibilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionFeasibilities_Drugs_DrugId",
                        column: x => x.DrugId,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DrugNeedAssessments_DrugId",
                table: "DrugNeedAssessments",
                column: "DrugId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DrugScores_DrugId",
                table: "DrugScores",
                column: "DrugId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductionFeasibilities_DrugId",
                table: "ProductionFeasibilities",
                column: "DrugId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrugNeedAssessments");

            migrationBuilder.DropTable(
                name: "DrugScores");

            migrationBuilder.DropTable(
                name: "ProductionFeasibilities");

            migrationBuilder.DropTable(
                name: "Drugs");
        }
    }
}
