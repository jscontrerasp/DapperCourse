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
