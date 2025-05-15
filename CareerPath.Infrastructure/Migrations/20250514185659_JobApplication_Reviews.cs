using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerPath.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class JobApplication_Reviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, drop the foreign key constraints that reference the Companies table
            migrationBuilder.DropForeignKey(
                name: "FK_Job_Companies_CompanyId",
                table: "Job");

            // Drop the primary key constraint
            migrationBuilder.DropPrimaryKey(
                name: "PK_Companies",
                table: "Companies");

            // Drop the existing IDENTITY column
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Companies");

            // Add the new string Id column
            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Companies",
                type: "nvarchar(450)",
                nullable: false);

            // Add primary key constraint for the new Id column
            migrationBuilder.AddPrimaryKey(
                name: "PK_Companies",
                table: "Companies",
                column: "Id");

            // Update the CompanyId in Job table
            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "Job",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // Create the JobApplications table
            migrationBuilder.CreateTable(
                name: "JobApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    userId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResumeUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoverLetterUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplications_AspNetUsers_userId",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Create the Reviews table
            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_userId",
                table: "JobApplications",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserProfileId",
                table: "Reviews",
                column: "UserProfileId");

            // Re-add the foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_Job_Companies_CompanyId",
                table: "Job",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_Job_Companies_CompanyId",
                table: "Job");

            // Drop the tables
            migrationBuilder.DropTable(
                name: "JobApplications");

            migrationBuilder.DropTable(
                name: "Reviews");

            // Drop the primary key constraint
            migrationBuilder.DropPrimaryKey(
                name: "PK_Companies",
                table: "Companies");

            // Drop the string Id column
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Companies");

            // Add back the int Id column with IDENTITY
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Companies",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            // Add back the primary key constraint
            migrationBuilder.AddPrimaryKey(
                name: "PK_Companies",
                table: "Companies",
                column: "Id");

            // Restore the CompanyId column type
            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Job",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            // Re-add the foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_Job_Companies_CompanyId",
                table: "Job",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
