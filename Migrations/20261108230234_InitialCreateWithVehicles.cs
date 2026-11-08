using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ViceNews.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithVehicles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleTags", x => x.Id);
                    table.CheckConstraint("CK_VehicleTags_Slug_Normalized", "\"Slug\" = lower(btrim(\"Slug\"))");
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    PriceGta = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ManufacturerId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    MaxSpeed = table.Column<short>(type: "smallint", nullable: false),
                    Acceleration = table.Column<short>(type: "smallint", nullable: false),
                    Braking = table.Column<short>(type: "smallint", nullable: false),
                    Handling = table.Column<short>(type: "smallint", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.CheckConstraint("CK_Vehicles_Acceleration", "\"Acceleration\" BETWEEN 0 AND 100");
                    table.CheckConstraint("CK_Vehicles_Braking", "\"Braking\" BETWEEN 0 AND 100");
                    table.CheckConstraint("CK_Vehicles_Handling", "\"Handling\" BETWEEN 0 AND 100");
                    table.CheckConstraint("CK_Vehicles_MaxSpeed", "\"MaxSpeed\" BETWEEN 0 AND 100");
                    table.CheckConstraint("CK_Vehicles_PriceGta", "\"PriceGta\" >= 0");
                    table.ForeignKey(
                        name: "FK_Vehicles_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicles_VehicleCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "VehicleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleTagAssignments",
                columns: table => new
                {
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    VehicleTagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleTagAssignments", x => new { x.VehicleId, x.VehicleTagId });
                    table.ForeignKey(
                        name: "FK_VehicleTagAssignments_VehicleTags_VehicleTagId",
                        column: x => x.VehicleTagId,
                        principalTable: "VehicleTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleTagAssignments_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Manufacturers_Name",
                table: "Manufacturers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Manufacturers_Slug",
                table: "Manufacturers",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleCategories_Name",
                table: "VehicleCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleCategories_Slug",
                table: "VehicleCategories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CategoryId",
                table: "Vehicles",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_ManufacturerId",
                table: "Vehicles",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Slug",
                table: "Vehicles",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTagAssignments_VehicleTagId",
                table: "VehicleTagAssignments",
                column: "VehicleTagId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTags_Slug",
                table: "VehicleTags",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleTagAssignments");

            migrationBuilder.DropTable(
                name: "VehicleTags");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Manufacturers");

            migrationBuilder.DropTable(
                name: "VehicleCategories");
        }
    }
}
