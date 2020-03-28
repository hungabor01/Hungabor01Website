using Hungabor01Website.BusinessLogic.Enums;
using Hungabor01Website.Database.Core.Entities;
using Hungabor01Website.Database.UnitOfWork;
using Hungabor01Website.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace Hungabor01Website.Tests.Database.Repositories
{
    public class AttachmentRepositoryTests : IDisposable
    {
        private readonly ConfigurationHelper _configurationHelper;
        private readonly ServiceProviderHelper _serviceProviderHelper;
        private readonly DatabaseHelper _databaseHelper;
        private readonly IdentityHelper _identityHelper;

        private readonly Attachment _testProfilePicture;

        private static readonly string _filename = "test";
        private static readonly string _extension = ".jpg";
        private static readonly byte[] _data = new byte[] { 0x00, 0x01, 0x0f, 0x69, 0xaa, 0xf0, 0xff };

        public AttachmentRepositoryTests()
        {
            _configurationHelper = new ConfigurationHelper();
            _serviceProviderHelper = new ServiceProviderHelper(_configurationHelper.Configuration);
            _databaseHelper = new DatabaseHelper(_configurationHelper.Configuration);
            _identityHelper = new IdentityHelper("AttachmentRepoTest");
            _identityHelper.AddTestUser(_databaseHelper.Context);

            _testProfilePicture = new Attachment
            {
                UserId = _identityHelper.TestUser.Id,
                Type = AttachmentType.ProfilePicture.ToString(),
                Filename = _filename,
                Extension = _extension,
                Data = _data
            };
        }

        [Fact]
        public async Task GetProfilePictureAsync_UserIsNull_ThrowArgumentNullExceptionAsync()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                try
                {
                    await unitOfWork.AttachmentRepository.GetProfilePictureAsync(null);

                    Assert.True(false, "No exception was thrown.");
                }
                catch (ArgumentNullException ex)
                {
                    Assert.Equal("user", ex.ParamName);
                }
                catch (Exception)
                {
                    Assert.True(false, "Wrong type of exception was thrown.");
                }
            }
        }

        [Fact]
        public async Task GetProfilePictureAsync_UserHasNoProfilePicture_ReturnNullAsync()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var data = await unitOfWork.AttachmentRepository.GetProfilePictureAsync(_identityHelper.TestUser);

                Assert.Null(data);
            }
        }

        [Fact]
        public async Task GetProfilePictureAsync_UserHasProfilePicture_ReturnDataAsync()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                unitOfWork.AttachmentRepository.Add(_testProfilePicture);
                unitOfWork.Complete();
            }

            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var result = await unitOfWork.AttachmentRepository.GetProfilePictureAsync(_identityHelper.TestUser);
                Assert.NotNull(result);

                var resultData = result.Value.Data;
                Assert.NotNull(resultData);
                Assert.Equal(_data, resultData);

                var resultExtension = result.Value.Extension;
                Assert.NotNull(resultExtension);
                Assert.Equal(_extension, resultExtension);
            }

            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                unitOfWork.AttachmentRepository.Remove(_testProfilePicture);
                unitOfWork.Complete();
            }
        }

        [Fact]
        public async Task ChangeProfilePictureAsync_UserIsNull_ThrowArgumentNullExceptionAsync()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                try
                {
                    var file = new Mock<IFormFile>().Object;
                    await unitOfWork.AttachmentRepository.UploadProfilePictureAsync(null, file);

                    Assert.True(false, "No exception was thrown.");
                }
                catch (ArgumentNullException ex)
                {
                    Assert.Equal("user", ex.ParamName);
                }
                catch (Exception)
                {
                    Assert.True(false, "Wrong type of exception was thrown.");
                }
            }
        }

        [Fact]
        public async Task ChangeProfilePictureAsync_FileIsNull_ThrowArgumentNullExceptionAsync()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                try
                {
                    await unitOfWork.AttachmentRepository.UploadProfilePictureAsync(_identityHelper.TestUser, null);

                    Assert.True(false, "No exception was thrown.");
                }
                catch (ArgumentNullException ex)
                {
                    Assert.Equal("file", ex.ParamName);
                }
                catch (Exception)
                {
                    Assert.True(false, "Wrong type of exception was thrown.");
                }
            }
        }

        [Fact]
        public async Task ChangeProfilePictureAsync_UserHasNoProfilePicture_NewEntryAddedToDatabaseAsync()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var profilePictures = unitOfWork.AttachmentRepository
                  .Find(a => a.UserId == _identityHelper.TestUser.Id && a.Type == AttachmentType.ProfilePicture.ToString())
                  .ToList();

                Assert.Equal(0, profilePictures?.Count);
            }

            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var ms = new MemoryStream(_data);
                var file = new FormFile(ms, 0, ms.Length, _filename + _extension, _filename + _extension);

                await unitOfWork.AttachmentRepository.UploadProfilePictureAsync(_identityHelper.TestUser, file);

                unitOfWork.Complete();
            }

            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var profilePictures = unitOfWork.AttachmentRepository
                  .Find(a => a.UserId == _identityHelper.TestUser.Id && a.Type == AttachmentType.ProfilePicture.ToString())
                  .ToList();

                Assert.Single(profilePictures);

                var profilePicture = profilePictures[0];
                Assert.Equal(_filename, profilePicture.Filename);
                Assert.Equal(_extension, profilePicture.Extension);
                Assert.Equal(_data, profilePicture.Data);
            }

            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var profilePicture = unitOfWork.AttachmentRepository
                  .SingleOrDefault(a => a.UserId == _identityHelper.TestUser.Id && a.Type == AttachmentType.ProfilePicture.ToString());

                unitOfWork.AttachmentRepository.Remove(profilePicture);
                unitOfWork.Complete();
            }
        }

        [Fact]
        public async Task ChangeProfilePictureAsync_UserHasProfilePicture_ProfilePictureIsOverriddenAsync()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var attachment = new Attachment
                {
                    UserId = _identityHelper.TestUser.Id,
                    Type = AttachmentType.ProfilePicture.ToString(),
                    Filename = "wrongFile",
                    Extension = ".txt",
                    Data = new byte[] { 0x15, 0xa1, 0xbf, 0x69, 0x05, 0xd4, 0xca }
                };

                unitOfWork.AttachmentRepository.Add(attachment);
                unitOfWork.Complete();
            }

            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var profilePictures = unitOfWork.AttachmentRepository
                  .Find(a => a.UserId == _identityHelper.TestUser.Id && a.Type == AttachmentType.ProfilePicture.ToString())
                  .ToList();
                Assert.Single(profilePictures);
            }

            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var ms = new MemoryStream(_data);
                var file = new FormFile(ms, 0, ms.Length, _filename + _extension, _filename + _extension);

                await unitOfWork.AttachmentRepository.UploadProfilePictureAsync(_identityHelper.TestUser, file);

                unitOfWork.Complete();
            }

            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var profilePictures = unitOfWork.AttachmentRepository
                  .Find(a => a.UserId == _identityHelper.TestUser.Id && a.Type == AttachmentType.ProfilePicture.ToString())
                  .ToList();

                Assert.Single(profilePictures);

                var profilePicture = profilePictures[0];
                Assert.Equal(_filename, profilePicture.Filename);
                Assert.Equal(_extension, profilePicture.Extension);
                Assert.Equal(_data, profilePicture.Data);
            }

            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var profilePicture = unitOfWork.AttachmentRepository
                  .SingleOrDefault(a => a.UserId == _identityHelper.TestUser.Id && a.Type == AttachmentType.ProfilePicture.ToString());

                unitOfWork.AttachmentRepository.Remove(profilePicture);
                unitOfWork.Complete();
            }
        }

        [Fact]
        public async Task DeleteProfilePictureAsync_UserIsNull_ThrowArgumentNullExceptionAsync()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                try
                {
                    await unitOfWork.AttachmentRepository.DeleteProfilePictureAsync(null);

                    Assert.True(false, "No exception was thrown.");
                }
                catch (ArgumentNullException ex)
                {
                    Assert.Equal("user", ex.ParamName);
                }
                catch (Exception)
                {
                    Assert.True(false, "Wrong type of exception was thrown.");
                }
            }
        }

        [Fact]
        public async Task DeleteProfilePictureAsync_UserHasNoProfilePicture_ReturnFalseAsync()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var result = await unitOfWork.AttachmentRepository.DeleteProfilePictureAsync(_identityHelper.TestUser);
                unitOfWork.Complete();
                Assert.False(result);
            }
        }

        [Fact]
        public async Task DeleteProfilePicture_UserHasProfilePicture_ReturnTrueAsync()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                unitOfWork.AttachmentRepository.Add(_testProfilePicture);
                unitOfWork.Complete();
            }

            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var result = await unitOfWork.AttachmentRepository.DeleteProfilePictureAsync(_identityHelper.TestUser);
                unitOfWork.Complete();
                Assert.True(result);
            }
        }

        public void Dispose()
        {
            _identityHelper.RemoveTestUser(_databaseHelper.Context);
            _databaseHelper.Dispose();
        }
    }
}