﻿using System;

namespace BloodDonation.Domain.Entities
{
    public class Employee : Entity
    {
        public string EmployeeOfficeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string DesignationId { get; set; }
        public string DepartmentId { get; set; }
        public string PhoneNumber { get; set; }
        public string ImageUrl { get; set; }
        public DateTime JoiningDate { get; set; }
        public DateTime LeavingDate { get; set; }
        public bool CurrentEmployee { get; set; } = true;
        public bool IsUser { get; set; } = false;
        public string UserName { get; set; }
        public string Address { get; set; }
    }
}