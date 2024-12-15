﻿using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Employee Number")]
        public int EmployeeNo { get; set; }  

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
