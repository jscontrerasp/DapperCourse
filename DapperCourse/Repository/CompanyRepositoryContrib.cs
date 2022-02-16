using Dapper;
using Dapper.Contrib.Extensions;
using DapperCourse.Data;
using DapperCourse.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperCourse.Repository
{
    public class CompanyRepositoryContrib : ICompanyRepository
    {
        //use the connection string directly
        private IDbConnection db;
        public CompanyRepositoryContrib(IConfiguration configuration)
        {
            this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public Company Add(Company company)
        {
            var id = db.Insert(company);
            company.CompanyId = (int)id;
            return company;
        }

        public Company Find(int id) => db.Get<Company>(id);

        public List<Company> GetAll() => db.GetAll<Company>().ToList();


        public void Remove(int id) => db.Delete(new Company {CompanyId = id});

        public Company Update(Company companyToUpdate)
        {
            db.Update(companyToUpdate);
            return companyToUpdate;
        }
    }
}
