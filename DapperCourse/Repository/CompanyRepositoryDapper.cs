using Dapper;
using DapperCourse.Data;
using DapperCourse.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperCourse.Repository
{
    public class CompanyRepositoryDapper : ICompanyRepository
    {
        //use the connection string directly
        private IDbConnection db;
        public CompanyRepositoryDapper(IConfiguration configuration)
        {
            this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public Company Add(Company company)
        {
            var sql = "INSERT INTO Companies (Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);"
                + "SELECT CAST(SCOPE_IDENTITY() as int);";
            //var id = db.Query<int>(sql, new
            //{
            //    company.Name,
            //    company.Address,
            //    company.City,
            //    company.State,
            //    company.PostalCode
            //}).Single();
            var id = db.Query<int>(sql,company).Single();
            company.CompanyId = id; 
            return company;
        }

        public Company Find(int id)
        {
            //never do this!
            //var sql = "SELECT * FROM Companies WHERE CompanyId = "+id;
            var sql = "SELECT * FROM Companies WHERE CompanyId = @CompanyId";
            return db.Query<Company>(sql,new { @CompanyId=id}).Single();
        }

        public List<Company> GetAll()
        {
            var sql = "SELECT * FROM Companies";
            return db.Query<Company>(sql).ToList();
        }

        public void Remove(int id)
        {
            var sql = "DELETE FROM Companies WHERE CompanyId = @Id";
            db.Execute( sql, new { id } );

        }

        public Company Update(Company companyToUpdate)
        {
            var sql = "UPDATE Companies SET Name = @Name, Address = @Address, " +
                "City = @City, State = @State, PostalCode = @PostalCode WHERE CompanyId = @CompanyId";
            //all the fields must match between companyToUpdate and the table, for use directly the object in the parameters
            //execute is used when you don't need to know the result
            db.Execute(sql, companyToUpdate);
            return companyToUpdate;
        }
    }
}
