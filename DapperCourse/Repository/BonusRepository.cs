using Dapper;
using DapperCourse.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Transactions;

namespace DapperCourse.Repository
{
    public class BonusRepository : IBonusRepository
    {
        private IDbConnection db;

        public BonusRepository(IConfiguration configuration)
        {
            this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public void AddTestCompanyWithEmployees(Company company)
        {
            var sql = "INSERT INTO Companies (Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);"
                + "SELECT CAST(SCOPE_IDENTITY() as int);";
            var id = db.Query<int>(sql, company).Single();
            company.CompanyId = id;

            //foreach (var employee in company.Employees)
            //{
            //    employee.CompanyId = company.CompanyId;
            //    var sqlEmp = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);"
            //    + "SELECT CAST(SCOPE_IDENTITY() as int);";
            //    db.Query<int>(sqlEmp, employee).Single();
            //}

            company.Employees.Select(emp => { emp.CompanyId = id ; return emp }).ToList();
            var sqlEmp = "INSERT INTO Employees(Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId); "
                + "SELECT CAST(SCOPE_IDENTITY() as int);";
            //bulk insert
            db.Execute(sqlEmp, company.Employees);
        }

        public void AddTestCompanyWithEmployeesWithTransaction(Company company)
        {
            using(var transaction = new TransactionScope())
            {
                try
                {
                    var sql = "INSERT INTO Companies (Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);"
                        + "SELECT CAST(SCOPE_IDENTITY() as int);";
                    var id = db.Query<int>(sql, company).Single();
                    company.CompanyId = id;

                    company.Employees.Select(emp => { emp.CompanyId = id; return emp }).ToList();
                    var sqlEmp = "INSERT INTO Employees(Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId); "
                        + "SELECT CAST(SCOPE_IDENTITY() as int);";
                    //bulk insert
                    db.Execute(sqlEmp, company.Employees);
                    transaction.Complete();
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            
        }

        public List<Company> GetAllCompaniesWithEmployees()
        {
            //CHECK THE ORDER OF C.*,E.*
            var sql = "SELECT C.*,E.* FROM Employees AS E INNER JOIN Companies AS C ON E.CompanyId = C.CompanyId";
            var companyDic = new Dictionary<int, Company>();
            var company = db.Query<Company,Employee,Company>(sql, ( comp , emp ) =>
            {
                if(!companyDic.TryGetValue(comp.CompanyId, out var currentCompany)){
                    currentCompany = comp;
                    companyDic.Add(currentCompany.CompanyId, currentCompany);
                }
                currentCompany.Employees.Add(emp);
                return currentCompany;
            },
            // SplitOn is the name of the column to separate the colums and create the differents objects
            splitOn: "EmployeeId");

            return company.Distinct().ToList(); 
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
            //CHECK THE ORDER OF E.*,C.*
            var sql = "SELECT E.*,C.* FROM Employees AS E INNER JOIN Companies AS C ON E.CompanyId = C.CompanyId";
            if (id != 0) sql += " WHERE E.CompanyId = @Id;";

            var employee = db.Query<Employee, Company, Employee>(sql,(emplo,comp) =>
            {
                emplo.Company = comp;
                return emplo;
            },
            new {id=id},
            //SplitOn is the name of the column to separate the colums and create the differents objects
            splitOn:"CompanyId");

            return employee.ToList();
        }

        public void RemoveRange(int[] companyId)
        {
            db.Query("DELETE FROM Companies WHERE CompanyId IN @listIds;", new { @listIds = companyId });
        }

        public List<Company> FilterCompanyByName(string name)
        {
            return db.Query<Company>("SELECT * FROM Companies WHERE Name like '%' + @name + '%'", new {name}).ToList();
        }
    }
}
