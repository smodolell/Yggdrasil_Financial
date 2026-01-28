using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yggdrasil.Quotation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InterestRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RateName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AnnualPercentage = table.Column<decimal>(type: "decimal(8,4)", precision: 8, scale: 4, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentFrequencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DaysInterval = table.Column<int>(type: "INTEGER", nullable: false),
                    PeriodsPerYear = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentFrequencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTerms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    NumberOfPayments = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTerms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuoteEngines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    FrequencyId = table.Column<int>(type: "INTEGER", nullable: false),
                    Term = table.Column<int>(type: "INTEGER", nullable: false),
                    Rate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    TaxRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    InsuranceRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    InsurancePercentage = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteEngines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Percentage = table.Column<decimal>(type: "TEXT", precision: 6, scale: 3, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AmortizationSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    QuoteEngineId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PeriodNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Principal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    Interest = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    InterestTax = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    TotalDue = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    PrincipalBalance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    InterestBalance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    InterestTaxBalance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    TotalBalance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmortizationSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AmortizationSchedules_QuoteEngines_QuoteEngineId",
                        column: x => x.QuoteEngineId,
                        principalTable: "QuoteEngines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    MinAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    MaxAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    MinAge = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxAge = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TaxRateId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plans_TaxRates_TaxRateId",
                        column: x => x.TaxRateId,
                        principalTable: "TaxRates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PlanPaymentTerms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlanId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentTermId = table.Column<int>(type: "INTEGER", nullable: false),
                    InterestRateId = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanPaymentTerms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanPaymentTerms_InterestRates_InterestRateId",
                        column: x => x.InterestRateId,
                        principalTable: "InterestRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanPaymentTerms_PaymentTerms_PaymentTermId",
                        column: x => x.PaymentTermId,
                        principalTable: "PaymentTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanPaymentTerms_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AmortizationSchedules_DueDate",
                table: "AmortizationSchedules",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_AmortizationSchedules_PeriodNumber",
                table: "AmortizationSchedules",
                column: "PeriodNumber");

            migrationBuilder.CreateIndex(
                name: "IX_AmortizationSchedules_QuoteEngineId",
                table: "AmortizationSchedules",
                column: "QuoteEngineId");

            migrationBuilder.CreateIndex(
                name: "IX_AmortizationSchedules_QuoteEngineId_PeriodNumber",
                table: "AmortizationSchedules",
                columns: new[] { "QuoteEngineId", "PeriodNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterestRates_Active_DateRange",
                table: "InterestRates",
                columns: new[] { "IsActive", "EffectiveDate", "ExpirationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_InterestRates_EffectiveDate",
                table: "InterestRates",
                column: "EffectiveDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterestRates_ExpirationDate",
                table: "InterestRates",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_InterestRates_IsActive",
                table: "InterestRates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_InterestRates_RateName",
                table: "InterestRates",
                column: "RateName");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentFrequencies_Active_DaysInterval",
                table: "PaymentFrequencies",
                columns: new[] { "IsActive", "DaysInterval" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentFrequencies_Code",
                table: "PaymentFrequencies",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentFrequencies_IsActive",
                table: "PaymentFrequencies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentFrequencies_Name",
                table: "PaymentFrequencies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTerms_Code",
                table: "PaymentTerms",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTerms_IsActive",
                table: "PaymentTerms",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTerms_Name",
                table: "PaymentTerms",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTerms_NumberOfPayments",
                table: "PaymentTerms",
                column: "NumberOfPayments",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanPaymentTerms_InterestRateId",
                table: "PlanPaymentTerms",
                column: "InterestRateId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanPaymentTerms_Order",
                table: "PlanPaymentTerms",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_PlanPaymentTerms_PaymentTermId",
                table: "PlanPaymentTerms",
                column: "PaymentTermId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanPaymentTerms_PlanId",
                table: "PlanPaymentTerms",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanPaymentTerms_PlanId_Order",
                table: "PlanPaymentTerms",
                columns: new[] { "PlanId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_PlanPaymentTerms_PlanId_PaymentTermId",
                table: "PlanPaymentTerms",
                columns: new[] { "PlanId", "PaymentTermId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plans_AgeRange",
                table: "Plans",
                columns: new[] { "MinAge", "MaxAge" });

            migrationBuilder.CreateIndex(
                name: "IX_Plans_AmountRange",
                table: "Plans",
                columns: new[] { "MinAmount", "MaxAmount" });

            migrationBuilder.CreateIndex(
                name: "IX_Plans_DateRange",
                table: "Plans",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Plans_EndDate",
                table: "Plans",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_Name",
                table: "Plans",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_StartDate",
                table: "Plans",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_TaxRateId",
                table: "Plans",
                column: "TaxRateId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteEngines_Amount",
                table: "QuoteEngines",
                column: "Amount");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteEngines_FrequencyId",
                table: "QuoteEngines",
                column: "FrequencyId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteEngines_Status",
                table: "QuoteEngines",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteEngines_Status_Frequency_Term",
                table: "QuoteEngines",
                columns: new[] { "Status", "FrequencyId", "Term" });

            migrationBuilder.CreateIndex(
                name: "IX_QuoteEngines_Term",
                table: "QuoteEngines",
                column: "Term");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_Active_DateRange",
                table: "TaxRates",
                columns: new[] { "IsActive", "EffectiveDate", "ExpirationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_Code",
                table: "TaxRates",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_EffectiveDate",
                table: "TaxRates",
                column: "EffectiveDate");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_ExpirationDate",
                table: "TaxRates",
                column: "ExpirationDate",
                filter: "[ExpirationDate] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_IsActive",
                table: "TaxRates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_Name",
                table: "TaxRates",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_Percentage",
                table: "TaxRates",
                column: "Percentage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AmortizationSchedules");

            migrationBuilder.DropTable(
                name: "PaymentFrequencies");

            migrationBuilder.DropTable(
                name: "PlanPaymentTerms");

            migrationBuilder.DropTable(
                name: "QuoteEngines");

            migrationBuilder.DropTable(
                name: "InterestRates");

            migrationBuilder.DropTable(
                name: "PaymentTerms");

            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.DropTable(
                name: "TaxRates");
        }
    }
}
