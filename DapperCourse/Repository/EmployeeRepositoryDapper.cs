using Dapper;
using DapperCourse.Data;
using DapperCourse.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperCourse.Repository
{
    public class EmployeeRepositoryDapper : IEmployeeRepository
    {
        //use the connection string directly
        private IDbConnection db;
        public EmployeeRepositoryDapper(IConfiguration configuration)
        {
            this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public Employee Add(Employee employee)
        {
            var sql = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);"
                + "SELECT CAST(SCOPE_IDENTITY() as int);";
            var id = db.Query<int>(sql,employee).Single();
            employee.EmployeeId = id; 
            return employee;
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            var sql = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);"
                + "SELECT CAST(SCOPE_IDENTITY() as int);";
            var id = await db.QueryAsync<int>(sql, employee);
            employee.EmployeeId = id.Single();
            return employee;
        }

        public Employee Find(int id)
        {
            var sql = "SELECT * FROM Employees WHERE EmployeeId = @Id";
            return db.Query<Employee>(sql,new { id }).Single();
        }

        public List<Employee> GetAll()
        {
            var sql = "SELECT * FROM Employees";
            return db.Query<Employee>(sql).ToList();
        }

        public void Remove(int id)
        {
            var sql = "DELETE FROM Employees WHERE EmployeeId = @Id";
            db.Execute( sql, new { id } );

        }

        public Employee Update(Employee employeeToUpdate)
        {
            var sql = "UPDATE Employees SET Name = @Name, Title = @Title, Email = @Email, " +
                "Phone = @Phone, CompanyId = @CompanyId WHERE EmployeeId = @EmployeeId";
            //all the fields must match between companyToUpdate and the table, for use directly the object in the parameters
            //execute is used when you don't need to know the result
            db.Execute(sql, employeeToUpdate);
            return employeeToUpdate;
        }
    }
}
