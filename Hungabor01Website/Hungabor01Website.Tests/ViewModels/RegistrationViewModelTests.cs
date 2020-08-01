using Hungabor01Website.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Hungabor01Website.Tests.ViewModels
{
    public class RegistrationViewModelTests
    {
        [Theory]
        [InlineData("hungabor01", "hungabor01@gmail.com", "Password123", "Password123")]
        public void IsModelStateValid_ValidData(string username, string email, string password, string confirmPassword)
        {
            var model = new RegistrationViewModel
            {
                Username = username,
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isModelStateValid);
        }

        [Theory]
        [InlineData("hungabor01", "hungabor01@gmail.com", "Password123", "Password789")]
        [InlineData("hungabor01", "hungabor01@gmail.com", "Password789", "Password123")]
        [InlineData("hungabor01", "hungabor01@gmail.com", "", "")]
        [InlineData("", "hungabor01@gmail.com", "Password123", "Password123")]
        [InlineData("hungabor01", "", "Password123", "Password123")]
        [InlineData("hungabor01", "email", "Password123", "Password123")]
        public void IsModelStateValid_InvalidData(string username, string email, string password, string confirmPassword)
        {
            var model = new RegistrationViewModel
            {
                Username = username,
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isModelStateValid);
        }
    }
}