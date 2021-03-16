using CompanyEmployees.MVC.Test;
using Entities.Models;
using NUnit.Framework;
using System;
using System.Linq;

public class TestRepositoryContextFactoryTests
    {
        [Test]
        public void Add_ShouldAddNewCompanyAndNewEmployee()
        {
            using (var factory = new TestRepositoryContextFactory())
            {
                Guid testEmployeeId = Guid.NewGuid();
                Guid testCompanyId = Guid.NewGuid();
                Company testCompany = new Company()
                {
                    Id = testCompanyId,
                    Name = "Test bedrijf",
                    Country = "Test land",
                    Description = "Test beschrijving",
                    Size = CompanySize.Small,
                    LaunchDate = DateTime.Today,
                    Address = "Test adres"
                };

                Employee testEmployee = new Employee()
                {
                    Id = testEmployeeId,
                    CompanyId = testCompanyId,
                    Name = "Jos",
                    Description = "Test employee",
                    Age = 45,
                    Gender = GeslachtType.Man,
                    Position = "Developer"
                };
                // Get a context
                using (var context = factory.CreateContext())
                {
                    context.Companies.Add(testCompany);
                    context.Employees.Add(testEmployee);
                    context.SaveChanges();
                }

                // Get another context using the same connection
                using (var context = factory.CreateContext())
                {
                    var count = context.Employees.Count();
                    Assert.AreEqual(4, count);

                    var emp = context.Employees.FirstOrDefault(e => e.Id == testEmployeeId);
                    Assert.IsNotNull(emp);
                }
            }
        }

    }

