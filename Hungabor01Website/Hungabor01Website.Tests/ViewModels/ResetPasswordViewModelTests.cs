using Hungabor01Website.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Hungabor01Website.Tests.ViewModels
{
    public class ResetPasswordViewModelTests
    {
        [Theory]
        [InlineData("hungabor01@gmail.com", "Password123", "Password123", "adf89234r5fbw")]
        public void IsModelStateValid_ValidData(string email, string password, string confirmPassword, string token)
        {
            var model = new ResetPasswordViewModel
            {
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword,
                Token = token
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isModelStateValid);
        }

        [Theory]
        [InlineData("hungabor01gmail.com", "Password123", "Password123", "")]
        [InlineData("hungabor01@gmail.com", "Password123", "Password789", "")]
        [InlineData("hungabor01@gmail.com", "Password789", "Password123", "")]
        [InlineData("hungabor01@gmail.com", "", "", "")]

        public void IsModelStateValid_InvalidData(string email, string password, string confirmPassword, string token)
        {
            var model = new ResetPasswordViewModel
            {
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword,
                Token = token
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isModelStateValid);
        }
    }
}