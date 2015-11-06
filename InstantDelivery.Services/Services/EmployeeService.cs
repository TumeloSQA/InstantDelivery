﻿using InstantDelivery.Core;
using InstantDelivery.Core.Entities;
using System;
using System.ComponentModel;
using System.Linq;

namespace InstantDelivery.Services
{
    public class EmployeeService : IDisposable
    {
        private readonly InstantDeliveryContext context = new InstantDeliveryContext();

        public string LastNameFilter { get; set; } = "";
        public string FirstNameFilter { get; set; } = "";
        public string EmailFilter { get; set; } = "";
        public EmployeeSortingProperty SortingProperty { get; set; } = EmployeeSortingProperty.ByFirstName;
        public int Total { get; set; }

        public IQueryable<Employee> GetAll()
        {
            Total = context.Employees.Count();
            return context.Employees;
        }

        public void Reload(Employee employee)
        {
            context.Entry(employee).Reload();
        }

        public void RemoveEmployee(Employee employee)
        {
            context.Employees.Remove(employee);
            context.SaveChanges();
        }

        public IQueryable<Employee> Page(int pageNumber, int pageSize)
        {
            // mozna poifować zeby sql'a skrócić ewentualnie?
            var tmp = context.Employees.Where(e => FirstNameFilter == "" || e.FirstName.StartsWith(FirstNameFilter))
                .Where(e => LastNameFilter == "" || e.LastName.StartsWith(LastNameFilter))
                .Where(e => EmailFilter == "" || e.Email.StartsWith(EmailFilter));
            if (SortingProperty == EmployeeSortingProperty.ByFirstName)
            {
                tmp = tmp.OrderBy(e => e.FirstName);
            }
            else if (SortingProperty == EmployeeSortingProperty.ByLastName)
            {
                tmp = tmp.OrderBy(e => e.LastName);
            }
            else
            {
                tmp = tmp.OrderBy(e => e.Id);
            }
            Total = tmp.Count();

            return tmp.Skip(pageSize * (pageNumber - 1))
            .Take(pageSize);
        }

        public void AddEmployee(Employee employee)
        {
            context.Employees.Add(employee);
            context.SaveChanges();
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose()
        {
            context.Dispose();
        }

    }

    public enum EmployeeSortingProperty
    {
        [Description("Po nazwisku")]
        ByLastName,
        [Description("Po imieniu")]
        ByFirstName,
        [Description("Po ID")]
        ByID
    }
}
