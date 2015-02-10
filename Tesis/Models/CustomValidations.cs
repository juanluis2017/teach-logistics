﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tesis.DAL;
using Tesis.Models;

namespace Tesis
{
    public class UniqueUser : ValidationAttribute 
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public UniqueUser(): base("{0} se encuentra registrado en el sistema")
        {

        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            User user = db.Users.Where(x => x.UserName.Equals((string)value) || x.IdCard.Equals((string)value)).FirstOrDefault();
            if (user == null)
            {
                return ValidationResult.Success;
            }
            else
            {
                var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(errorMessage);
            }
        }
    }

    public class DateValidation : ValidationAttribute
    {
        public DateValidation()
            : base("{0} es incorrecta")
        {

        }

        protected override ValidationResult IsValid(object value, ValidationContext validationcontext)
        {
            DateTime date;

            if (DateTime.TryParseExact((string)value, "dd/MM/yyyy", null, DateTimeStyles.None, out date) == true)
                return ValidationResult.Success;
            else
            {
                var errorMessage = FormatErrorMessage(validationcontext.DisplayName);
                return new ValidationResult(errorMessage);
            }
        }
    }
}