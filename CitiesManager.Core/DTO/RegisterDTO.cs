using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime;

namespace CitiesManager.Core.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Person Name can't be blank")]
        public string PersonName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email should be in proper email address format")]
        [Remote(action: "isEmailAlreadyRegistered", controller: "Account", ErrorMessage = "Email is already in use")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone Number can't be blank")]
        [RegularExpression("^[0-9]*$",ErrorMessage = "Phone number should contain digit only")]
        [Remote(action: "isPhoneAlreadyRegister", controller: "Account", ErrorMessage = "Phone Number is already in use")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password can't be blank")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password can't be blank")]
        [Compare("Password",ErrorMessage = "Password and Confirm password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

}
