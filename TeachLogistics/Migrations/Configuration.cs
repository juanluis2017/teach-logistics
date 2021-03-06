using System.Data.Entity.Migrations;
using TeachLogistics.Business;
using TeachLogistics.Models;

namespace TeachLogistics.DAL
{

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "TeachLogistics.DAL.ApplicationDbContext";
            // register mysql code generator
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
            SetHistoryContextFactory("MySql.Data.MySqlClient", (conn, schema) => new MySqlHistoryContext(conn, schema));
        }

        protected override void Seed(ApplicationDbContext context)
        {
            UserBL.CreateFirstUser(context);
            Semester semester = SemesterBL.CreateSemesterSection();
            context.Semesters.AddOrUpdate(semester);
            context.Products.AddRange(ProductBL.GetProducts());
            context.SaveChanges();
        }
    }
}
