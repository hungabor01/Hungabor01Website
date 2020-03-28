using Hungabor01Website.BusinessLogic;
using System;
using System.Collections.Generic;
using Xunit;

namespace Hungabor01Website.Tests.BusinessLogic
{
    public class ExtensionMethodsTests
    {
        [Fact]
        public void ThrowExceptionIfNull_ValidData_NotThrowException()
        {
            var objects = new List<object>
            {
                new object(),
                string.Empty
            };

            foreach (var obj in objects)
            {
                obj.ThrowExceptionIfNull(nameof(obj));
            }
        }

        [Theory]
        [InlineData(null)]
        public void ThrowExceptionIfNull_InvalidData_ThrowArgumentNullException(object obj)
        {
            try
            {
                obj.ThrowExceptionIfNull(nameof(obj));
                Assert.True(false, "No exception was thrown.");
            }
            catch (ArgumentNullException ex)
            {
                Assert.Equal(nameof(obj), ex.ParamName);
            }
            catch (Exception)
            {
                Assert.True(false, "Wrong type of exception was thrown.");
            }
        }

        [Theory]
        [InlineData("some string")]
        [InlineData("  a   ")]
        public void ThrowExceptionIfNullOrWhiteSpace_ValidData_NotThrowException(string str)
        {
            str.ThrowExceptionIfNullOrWhiteSpace(nameof(str));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\n")]
        public void ThrowExceptionIfNullOrWhiteSpace_InvalidData_ThrowArgumentException(string str)
        {
            try
            {
                str.ThrowExceptionIfNullOrWhiteSpace(nameof(str));
                Assert.True(false, "No exception was thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.Equal(nameof(str), ex.Message);
            }
            catch (Exception)
            {
                Assert.True(false, "Wrong type of exception was thrown.");
            }
        }

        [Fact]
        public void ThrowExceptionIfNullOrWhiteSpace_EmptyData_ThrowArgumentException()
        {
            string str;
            try
            {
                str = string.Empty;
                str.ThrowExceptionIfNullOrWhiteSpace(nameof(str));
                Assert.True(false, "No exception was thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.Equal(nameof(str), ex.Message);
            }
            catch (Exception)
            {
                Assert.True(false, "Wrong type of exception was thrown.");
            }
        }
    }
}
