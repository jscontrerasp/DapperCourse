using Dapper;
using DapperCourse.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperCourse.Repository
{
    public class BonusRepository : IBonusRepository
    {
        private IDbConnection db;

        public BonusRepository(IConfiguration configuration)
        {
            this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public Company GetCompanyWithEmployees(int id)
        {
            var p = new { CompanyId = id };
            var sql = "SELECT * FROM Companies WHERE CompanyId = @CompanyId;" +
                "SELECT *  FROM Employees WHERE CompanyId = @CompanyId;";

            Company company;

            using (var result = db.QueryMultiple(sql,p))
            {
                company = result.Read<Company>().ToList().FirstOrDefault();
                company.Employees = result.Read<Employee>().ToList();
            }

            return company;

        }

        public List<Employee> GetEmployeeWithCompany(int id)
        {
            var sql = "SELECT E.*,C.* FROM Employees AS E INNER JOIN Companies AS C ON E.CompanyId = C.CompanyId";
            if (id != 0) sql += " WHERE E.CompanyId = @Id;";

            var employee = db.Query<Employee, Company, Employee>(sql,(emplo,comp) =>
            {
                emplo.Company = comp;
                return emplo;
            },new {id=id},splitOn:"CompanyId");

            return employee.ToList();
        }
    }
}
