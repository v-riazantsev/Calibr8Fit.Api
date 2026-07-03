using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calibr8Fit.Api.Migrations
{
    /// <inheritdoc />
    public partial class MessagesRead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "read_at",
                table: "chat_messages");

            migrationBuilder.CreateTable(
                name: "chat_message_reads",
                columns: table => new
                {
                    chat_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chat_message_reads", x => new { x.chat_message_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_chat_message_reads_chat_messages_chat_message_id",
                        column: x => x.chat_message_id,
                        principalTable: "chat_messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_chat_message_reads_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_chat_message_reads_chat_message_id",
                table: "chat_message_reads",
                column: "chat_message_id");

            migrationBuilder.CreateIndex(
                name: "ix_chat_message_reads_user_id",
                table: "chat_message_reads",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chat_message_reads");

            migrationBuilder.AddColumn<DateTime>(
                name: "read_at",
                table: "chat_messages",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
