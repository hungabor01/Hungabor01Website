using Hungabor01Website.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Hungabor01Website.Tests.ViewModels
{
    public class ForgotPasswordViewModelTests
    {
        [Theory]
        [InlineData("hungabor01@gmail.com")]
        public void IsModelStateValid_ValidData(string email)
        {
            var model = new ForgotPasswordViewModel
            {
                Email = email
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isModelStateValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("hungabor01gmail.com")]
        public void IsModelStateValid_InvalidData(string email)
        {
            var model = new ForgotPasswordViewModel
            {
                Email = email
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isModelStateValid);
        }
    }
}