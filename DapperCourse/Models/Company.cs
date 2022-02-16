using Dapper.Contrib.Extensions;

namespace DapperCourse.Models
{
    //Reference to Companies table to Dapper Contrib
    [Table("Companies")]
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }

        [Write(false)]
        public List<Employee>? Employees { get; set; }
    }
}
