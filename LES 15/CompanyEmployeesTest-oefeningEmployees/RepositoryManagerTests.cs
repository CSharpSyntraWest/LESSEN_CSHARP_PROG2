using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.MVC.Test
{
    public class RepositoryManagerTests
    {

        #region EmployeesRepoTests//EMPLOYEES TESTS 

        [Test]
        public void GetAllEmployees_ShouldReturnAllEmployeesFromContext()
        {
            using (var factory = new TestRepositoryContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    var countEmployeesInDb = context.Employees.Count();

                    var repository = new RepositoryManager(context);
                    var emp = repository.Employee.GetAllEmployees(false);
                    Assert.IsNotNull(emp);
                    Assert.AreEqual(countEmployeesInDb, emp.Count());
                }
            }
        }

        [Test]
        public void GetEmployee_ShouldReturnEmployee()
        {
            //Arrange
            Guid testEmployeeId;
            using (var factory = new TestRepositoryContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    var testEmployee = context.Employees.FirstOrDefault();
                    testEmployeeId = testEmployee.Id;
                    var repository = new RepositoryManager(context);
                    //Act
                    var empl = repository.Employee.GetEmployee(testEmployeeId, false);
                    Assert.IsNotNull(empl);
                    Assert.AreEqual(testEmployeeId, empl.Id);
                }
            }
        }
        [Test]
        public void CreateEmployeeForExistingCompany_ShouldAddNewEmployeeToContextForCompany()
        {
            using (var factory = new TestRepositoryContextFactory())
            {
                //Arrange
                int count = 0;

                Company testCompany = null;
                Guid testEmployeeId = Guid.NewGuid();
                Employee testEmployee = new Employee()
                {
                    Id = testEmployeeId,
                    Name = "Jos",
                    Description = "Test employee",
                    Age = 45,
                    Gender = GeslachtType.Man,
                    Position = "Developer"
                };
                using (var context = factory.CreateContext())
                {
                    count = context.Employees.Count();
                    testCompany = context.Companies.FirstOrDefault();
                    testEmployee.CompanyId = testCompany.Id;
                    var repository = new RepositoryManager(context);
                    //Act
                    repository.Employee.CreateEmployeeForCompany(testCompany.Id, testEmployee);
                    repository.Save();
                }
                //Assert
                using (var context = factory.CreateContext())
                {
                    Assert.AreEqual(count + 1, context.Employees.Count());
                    var addedEmployee = context.Employees.Find(testEmployeeId);
                    Assert.IsNotNull(addedEmployee);
                    Assert.AreEqual(testEmployeeId, addedEmployee.Id);
                }
            }
        }

        [Test]
        public void SaveChangesGetEmployeeTrackChangesTrue_ShouldChangeEmployeeInContext()
        {
            using (var factory = new TestRepositoryContextFactory())
            {
                //Arrange          
                Guid testCompanyId;
                Guid testEmployeeId;
                Employee testEmployee;
               
                using (var context = factory.CreateContext())
                {
                    var repository = new RepositoryManager(context);
                    var firstCompany = context.Companies.FirstOrDefault();
                    testCompanyId = firstCompany.Id;
                    var firstEmployee = context.Employees.FirstOrDefault();
                    testEmployeeId = firstEmployee.Id;
                    //Act
                    testEmployee = repository.Employee.GetEmployee(testEmployeeId, true);

                    testEmployee.Name = "gewijzigde naam Joke";
                    testEmployee.Age = 18;
                    testEmployee.CompanyId =testCompanyId;
                    testEmployee.Description = "gewijzigde beschrijving";
                    testEmployee.Gender = GeslachtType.Vrouw;
                    testEmployee.Position = "gewijzigde positie";

                    repository.Save();
                }
                //Assert
                using (var context = factory.CreateContext())
                {
                    var changedEmployee = context.Employees.FirstOrDefault(e => e.Id == testEmployeeId);
                    Assert.IsNotNull(changedEmployee);
                    Assert.AreEqual(testEmployee.Id, changedEmployee.Id);
                    Assert.AreEqual(testEmployee.Name, changedEmployee.Name);
                    Assert.AreEqual(testEmployee.Age, changedEmployee.Age);
                    Assert.AreEqual(testEmployee.CompanyId, changedEmployee.CompanyId);
                    Assert.AreEqual(testEmployee.Description, changedEmployee.Description);
                    Assert.AreEqual(testEmployee.Gender, changedEmployee.Gender);
                    Assert.AreEqual(testEmployee.Position, changedEmployee.Position);
                }
            }
        }
        [Test]
        public void DeleteEmployee_ShouldRemoveEmployeeFromContext()
        {
            using (var factory = new TestRepositoryContextFactory())
            {
                //Arrange          
                Guid testEmployeeId;
                int count;
                using (var context = factory.CreateContext())
                {
                    count = context.Employees.Count();
                    var repository = new RepositoryManager(context);
                    var firstEmployee = context.Employees.FirstOrDefault();
                    testEmployeeId = firstEmployee.Id;
                    //Act
                    repository.Employee.DeleteEmployee(firstEmployee);

                    repository.Save();
                }
                //Assert
                using (var context = factory.CreateContext())
                {
                    Assert.AreEqual(count - 1, context.Employees.Count());
                    Assert.IsFalse(context.Employees.Where(c => c.Id == testEmployeeId).Any());
                }
            }
        }
        #endregion //EMPLOYEES TESTS
  
    }
}

