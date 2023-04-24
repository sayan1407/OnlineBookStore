using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess
{
    public partial class PopulateCategoriesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO CATEGORIES(NAME,DISPLAYORDER,CREATEDDATETIME) VALUES ('Biography',1,'01/01/2023')");
            

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM CATEGORIES WHERE NAME = 'Biography'");

        }
    }
}
