#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DapperCourse.Data;
using DapperCourse.Models;
using DapperCourse.Repository;

namespace DapperCourse.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ICompanyRepository _companyRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IBonusRepository _bonusRepo;

        [BindProperty]
        public Employee Employee { get; set; } 
        public EmployeesController(ICompanyRepository companyRepo,IEmployeeRepository employeeRepo, IBonusRepository bonusRepo)
        {
            _companyRepo = companyRepo;
            _employeeRepo = employeeRepo;
            _bonusRepo = bonusRepo;
        }

        // GET: Employees
        public async Task<IActionResult> Index(int companyId=0)
        {
            // N+1 query problem
            //List<Employee> employees = _employeeRepo.GetAll();
            //foreach (Employee employee in employees)
            //    employee.Company = _companyRepo.Find(employee.CompanyId);
            List<Employee> employees = _bonusRepo.GetEmployeeWithCompany(companyId);
            return View(employees);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = _employeeRepo.Find(id.GetValueOrDefault());
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> companyList = _companyRepo.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.CompanyId.ToString()
            });
            ViewBag.CompanyList = companyList;
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePOST()
        {
            if (ModelState.IsValid)
            {
                _employeeRepo.Add(Employee);
                return RedirectToAction(nameof(Index));
            }
            return View(Employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Employee = _employeeRepo.Find(id.GetValueOrDefault());
            IEnumerable<SelectListItem> companyList = _companyRepo.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.CompanyId.ToString()
            });
            ViewBag.CompanyList = companyList;
            if (Employee == null)
            {
                return NotFound();
            }
            return View(Employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (id != Employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _employeeRepo.Update(Employee);
                return RedirectToAction(nameof(Index));
            }
            return View(Employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _employeeRepo.Remove(id.GetValueOrDefault());
            return RedirectToAction(nameof(Index));
        }

    }
}
