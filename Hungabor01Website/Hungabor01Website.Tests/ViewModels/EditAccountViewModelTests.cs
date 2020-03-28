using Hungabor01Website.ViewModels;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Hungabor01Website.Tests.ViewModels
{
    public class EditAccountViewModelTests
    {
        [Theory]
        [InlineData("test.jpg", 4 * 1024 * 1024)]
        [InlineData("test.png", 4 * 1024 * 1024)]

        public void IsModelStateValid_ValidData(string fileName, int size)
        {
            var mock = new Mock<IFormFile>();
            mock.SetupGet(f => f.FileName).Returns(fileName);
            mock.SetupGet(f => f.Length).Returns(size);

            var model = new EditAccountViewModel
            {
                ProfilePicture = mock.Object
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isModelStateValid);
        }

        [Fact]
        public void IsModelStateValid_NullData()
        {
            var model = new EditAccountViewModel
            {
                ProfilePicture = null
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isModelStateValid);
        }

        [Theory]
        [InlineData("test.jpg", 6 * 1024 * 1024)]
        [InlineData("test", 4 * 1024 * 1024)]
        [InlineData("test.pdf", 4 * 1024 * 1024)]
        [InlineData("", 4 * 1024 * 1024)]
        public void IsModelStateValid_InvalidData(string fileName, int size)
        {
            var mock = new Mock<IFormFile>();
            mock.SetupGet(f => f.FileName).Returns(fileName);
            mock.SetupGet(f => f.Length).Returns(size);

            var model = new EditAccountViewModel
            {
                ProfilePicture = mock.Object
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isModelStateValid);
        }
    }
}