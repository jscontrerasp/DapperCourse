using Dapper;
using DapperCourse.Data;
using DapperCourse.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperCourse.Repository
{
    public class CompanyRepositorySP : ICompanyRepository
    {
        //use the connection string directly
        private IDbConnection db;
        public CompanyRepositorySP(IConfiguration configuration)
        {
            this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public Company Add(Company company)
        {
            //pass params to Store Procedures
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", 0, DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Name", company.Name);
            parameters.Add("@Address", company.Address);
            parameters.Add("@City", company.City);
            parameters.Add("@State", company.State);
            parameters.Add("@PostalCode", company.PostalCode);

            this.db.Execute("usp_AddCompany", parameters, commandType: CommandType.StoredProcedure);
            company.CompanyId = parameters.Get<int>("CompanyId");
            return company;
        }

        public Company Find(int id) => db.Query<Company>("usp_GetCompany", new { CompanyId= id} ,  commandType: CommandType.StoredProcedure).SingleOrDefault();

        public List<Company> GetAll() => db.Query<Company>("usp_GetAllCompany", commandType: CommandType.StoredProcedure).ToList();


        public void Remove(int id) => db.Query<Company>("usp_RemoveCompany", new { CompanyId = id }, commandType: CommandType.StoredProcedure);

        public Company Update(Company companyToUpdate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyToUpdate.CompanyId, DbType.Int32);
            parameters.Add("@Name", companyToUpdate.Name);
            parameters.Add("@Address", companyToUpdate.Address);
            parameters.Add("@City", companyToUpdate.City);
            parameters.Add("@State", companyToUpdate.State);
            parameters.Add("@PostalCode", companyToUpdate.PostalCode);
            this.db.Execute("usp_UpdateCompany", parameters, commandType: CommandType.StoredProcedure);
            return companyToUpdate;
        }
    }
}
