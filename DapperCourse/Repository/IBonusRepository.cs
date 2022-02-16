using DapperCourse.Models;

namespace DapperCourse.Repository
{
    public interface IBonusRepository
    {
        List<Employee> GetEmployeeWithCompany(int id);
    }
}
