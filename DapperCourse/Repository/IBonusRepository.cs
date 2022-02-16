using DapperCourse.Models;

namespace DapperCourse.Repository
{
    public interface IBonusRepository
    {
        List<Employee> GetEmployeeWithCompany(int id);

        Company GetCompanyWithEmployees(int id);

        List<Company> GetAllCompaniesWithEmployees();
    }
}
