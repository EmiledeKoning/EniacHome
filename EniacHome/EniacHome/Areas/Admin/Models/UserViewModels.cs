﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;

namespace EniacHome.Areas.Admin.Models
{
    public class UsersListViewModel
    {

        [Key]
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        [Display(Name = "Name")]
        [Required]
        public string FullName { get; set; }

        [Required]
        [StringLength(128, ErrorMessage = "Address cannot be longer than 128 characters.")]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.PostalCode)]
        [StringLength(128, ErrorMessage = "Postal code cannot be longer than 128 characters.")]
        [Display(Name = "Postal code")]
        public string PostalCode { get; set; }

        [Required]
        [StringLength(128, ErrorMessage = "Country cannot be longer than 128 characters.")]
        public string Country { get; set; }
    }

    public class UsersEditViewModel
    {
        [Key]
        [HiddenInput(DisplayValue = true)]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Username")]
        [HiddenInput(DisplayValue = true)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [HiddenInput(DisplayValue = true)]
        public string Email { get; set; }

        [Required]
        [HiddenInput(DisplayValue = true)]
        public bool EmailConfirmed { get; set; }

        [Required]
        [Display(Name = "Role")]
        public List<string> Roles { get; set; }

        [Required]
        [Display(Name = "Current Role")]
        [HiddenInput(DisplayValue = false)]
        public string CurrentRole { get; set; }

        [Display(Name = "Name")]
        [Required]
        [HiddenInput(DisplayValue = true)]
        public string FullName { get; set; }

        [Required]
        [StringLength(128, ErrorMessage = "Address cannot be longer than 128 characters.")]
        [HiddenInput(DisplayValue = true)]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.PostalCode)]
        [StringLength(128, ErrorMessage = "Postal code cannot be longer than 128 characters.")]
        [Display(Name = "Postal code")]
        [HiddenInput(DisplayValue = true)]
        public string PostalCode { get; set; }

        [Required]
        [StringLength(128, ErrorMessage = "Country cannot be longer than 128 characters.")]
        [HiddenInput(DisplayValue = true)]
        public string Country { get; set; }

        [StringLength(50, ErrorMessage = "Phonenumber cannot be longer than 50 characters")]
        [HiddenInput(DisplayValue = true)]
        public string PhoneNumber { get; set; }

        [Required]
        [HiddenInput(DisplayValue = true)]
        public bool PhoneNumberConfirmed { get; set; }

        [Required]
        [HiddenInput(DisplayValue = true)]
        public bool TwoFactorEnabled { get; set; }

        [Required]
        [HiddenInput(DisplayValue = true)]
        public int AccesFailedCount { get; set; }

        [Required]
        [HiddenInput(DisplayValue = true)]
        public bool LockedOutEnabled { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [HiddenInput(DisplayValue = true)]
        public DateTime LockedOutEndDateUtc { get; set; }
    }
}
