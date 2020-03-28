using Hungabor01Website.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Hungabor01Website.Tests.ViewModels
{
    public class LoginViewModelTests
    {
        [Theory]
        [InlineData("hungabor01", "Password123", true, "")]
        [InlineData("12345", "pwd", false, "")]
        public void IsModelStateValid_ValidData(string username, string password, bool rememberMe, string returnUrl)
        {
            var model = new LoginViewModel
            {
                UsernameOrEmail = username,
                Password = password,
                RememberMe = rememberMe,
                ReturnUrl = returnUrl
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isModelStateValid);
        }

        [Theory]
        [InlineData("", "Password123", true, "")]
        [InlineData("hungabor01", "", true, "")]
        public void IsModelStateValid_InvalidData(string username, string password, bool rememberMe, string returnUrl)
        {
            var model = new LoginViewModel
            {
                UsernameOrEmail = username,
                Password = password,
                RememberMe = rememberMe,
                ReturnUrl = returnUrl
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isModelStateValid);
        }
    }
}