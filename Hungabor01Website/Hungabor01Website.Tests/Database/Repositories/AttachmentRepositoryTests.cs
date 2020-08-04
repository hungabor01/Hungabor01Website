using Common.Enums;
using Database.Core.Entities;
using Database.UnitOfWork;
using Hungabor01Website.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Hungabor01Website.Tests.Database.Repositories
{
    public class AttachmentRepositoryTests : IDisposable
    {
        private readonly ConfigurationHelper _configurationHelper;
        private readonly ServiceProviderHelper _serviceProviderHelper;
        private readonly DatabaseHelper _databaseHelper;
        private readonly IdentityHelper _identityHelper;

        private readonly Attachment _testProfilePicture;

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
                Filename = "test",
                Extension = ".jpg",
                Data = new byte[] { 0x00, 0x01, 0x0f, 0x69, 0xaa, 0xf0, 0xff }
            };
        }

        [Fact]
        public async Task GetProfilePictureAsync_UserIsNull_ThrowArgumentNullException()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                try
                {
                    await unitOfWork.AttachmentRepository.GetProfilePictureAsync(null);

                    Assert.True(false, "No exception was thrown.");
                }
                catch (ArgumentException ex)
                {
                    Assert.Equal("userId", ex.Message);
                }
                catch (Exception)
                {
                    Assert.True(false, "Wrong type of exception was thrown.");
                }
            }
        }

        [Fact]
        public async Task GetProfilePictureAsync_UserHasNoProfilePicture_ReturnNull()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var data = await unitOfWork.AttachmentRepository.GetProfilePictureAsync(_identityHelper.TestUser.Id);

                Assert.Null(data);
            }
        }

        [Fact]
        public async Task GetProfilePictureAsync_UserHasProfilePicture_ReturnData()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                unitOfWork.AttachmentRepository.Add(_testProfilePicture);
                unitOfWork.Complete();
            }

            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var result = await unitOfWork.AttachmentRepository.GetProfilePictureAsync(_identityHelper.TestUser.Id);
                Assert.NotNull(result);

                var resultData = result.Value.Data;
                Assert.NotNull(resultData);
                Assert.Equal(_testProfilePicture.Data, resultData);

                var resultExtension = result.Value.Extension;
                Assert.NotNull(resultExtension);
                Assert.Equal(_testProfilePicture.Extension, resultExtension);
            }

            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                unitOfWork.AttachmentRepository.Remove(_testProfilePicture);
                unitOfWork.Complete();
            }
        }

        [Fact]
        public async Task UploadProfilePictureAsync_UserIsNull_ThrowArgumentNullException()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                try
                {
                    await unitOfWork.AttachmentRepository.UploadProfilePictureAsync(
                        null, _testProfilePicture.Filename + _testProfilePicture.Extension, _testProfilePicture.Data);

                    Assert.True(false, "No exception was thrown.");
                }
                catch (ArgumentException ex)
                {
                    Assert.Equal("userId", ex.Message);
                }
                catch (Exception)
                {
                    Assert.True(false, "Wrong type of exception was thrown.");
                }
            }
        }

        [Fact]
        public async Task UploadProfilePictureAsync_FileNameIsNull_ThrowArgumentNullException()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                try
                {
                    await unitOfWork.AttachmentRepository.UploadProfilePictureAsync(
                        _identityHelper.TestUser.Id, null, _testProfilePicture.Data);

                    Assert.True(false, "No exception was thrown.");
                }
                catch (ArgumentException ex)
                {
                    Assert.Equal("filename", ex.Message);
                }
                catch (Exception)
                {
                    Assert.True(false, "Wrong type of exception was thrown.");
                }
            }
        }

        [Fact]
        public async Task UploadProfilePictureAsync_FileDataIsNull_ThrowArgumentNullException()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                try
                {
                    await unitOfWork.AttachmentRepository.UploadProfilePictureAsync(
                        _identityHelper.TestUser.Id, _testProfilePicture.Filename + _testProfilePicture.Extension, null);

                    Assert.True(false, "No exception was thrown.");
                }
                catch (ArgumentNullException ex)
                {
                    Assert.Equal("fileData", ex.ParamName);
                }
                catch (Exception)
                {
                    Assert.True(false, "Wrong type of exception was thrown.");
                }
            }
        }

        [Fact]
        public async Task UploadProfilePictureAsync_UserHasNoProfilePicture_NewEntryAddedToDatabase()
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
                await unitOfWork.AttachmentRepository.UploadProfilePictureAsync(
                    _identityHelper.TestUser.Id, _testProfilePicture.Filename + _testProfilePicture.Extension, _testProfilePicture.Data);

                unitOfWork.Complete();
            }

            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var profilePictures = unitOfWork.AttachmentRepository
                  .Find(a => a.UserId == _identityHelper.TestUser.Id && a.Type == AttachmentType.ProfilePicture.ToString())
                  .ToList();

                Assert.Single(profilePictures);

                var profilePicture = profilePictures[0];
                Assert.Equal(_testProfilePicture.Filename, profilePicture.Filename);
                Assert.Equal(_testProfilePicture.Extension, profilePicture.Extension);
                Assert.Equal(_testProfilePicture.Data, profilePicture.Data);
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
        public async Task UploadProfilePictureAsync_UserHasProfilePicture_ProfilePictureIsOverridden()
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
                await unitOfWork.AttachmentRepository.UploadProfilePictureAsync(
                    _identityHelper.TestUser.Id, _testProfilePicture.Filename + _testProfilePicture.Extension, _testProfilePicture.Data);

                unitOfWork.Complete();
            }

            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var profilePictures = unitOfWork.AttachmentRepository
                  .Find(a => a.UserId == _identityHelper.TestUser.Id && a.Type == AttachmentType.ProfilePicture.ToString())
                  .ToList();

                Assert.Single(profilePictures);

                var profilePicture = profilePictures[0];
                Assert.Equal(_testProfilePicture.Filename, profilePicture.Filename);
                Assert.Equal(_testProfilePicture.Extension, profilePicture.Extension);
                Assert.Equal(_testProfilePicture.Data, profilePicture.Data);
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
        public async Task DeleteProfilePictureAsync_UserIsNull_ThrowArgumentNullException()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                try
                {
                    await unitOfWork.AttachmentRepository.DeleteProfilePictureAsync(null);

                    Assert.True(false, "No exception was thrown.");
                }
                catch (ArgumentException ex)
                {
                    Assert.Equal("userId", ex.Message);
                }
                catch (Exception)
                {
                    Assert.True(false, "Wrong type of exception was thrown.");
                }
            }
        }

        [Fact]
        public async Task DeleteProfilePictureAsync_UserHasNoProfilePicture_ReturnFalse()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var result = await unitOfWork.AttachmentRepository.DeleteProfilePictureAsync(_identityHelper.TestUser.Id);
                unitOfWork.Complete();
                Assert.False(result);
            }
        }

        [Fact]
        public async Task DeleteProfilePicture_UserHasProfilePicture_ReturnTrue()
        {
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                unitOfWork.AttachmentRepository.Add(_testProfilePicture);
                unitOfWork.Complete();
            }

            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var result = await unitOfWork.AttachmentRepository.DeleteProfilePictureAsync(_identityHelper.TestUser.Id);
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