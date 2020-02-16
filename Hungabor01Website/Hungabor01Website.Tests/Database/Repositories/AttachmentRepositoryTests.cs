using Autofac.Extras.Moq;
using Hungabor01Website.Database;
using Hungabor01Website.Database.Entities;
using Hungabor01Website.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace Hungabor01Website.Tests.Database.Repositories
{
  public class AttachmentRepositoryTests : IDisposable
  {
    private readonly ConfigurationHelper configurationHelper;
    private readonly ServiceProviderHelper serviceProviderHelper;
    private readonly DatabaseHelper databaseHelper;
    private readonly IdentityHelper identityHelper;

    private readonly Attachment testProfilePicture;

    private static readonly string filename = "test";
    private static readonly string extension = ".jpg";
    private static readonly byte[] data = new byte[] { 0x00, 0x01, 0x0f, 0x69, 0xaa, 0xf0, 0xff };

    public AttachmentRepositoryTests()
    {
      configurationHelper = new ConfigurationHelper();
      serviceProviderHelper = new ServiceProviderHelper(configurationHelper.Configuration);
      databaseHelper = new DatabaseHelper(configurationHelper.Configuration);
      identityHelper = new IdentityHelper("AttachmentRepoTest");
      identityHelper.AddTestUser(databaseHelper.Context);

      testProfilePicture = new Attachment
      {
        UserId = identityHelper.TestUser.Id,
        Type = AttachmentType.ProfilePicture,
        Filename = filename,
        Extension = extension,
        Data = data
      };
    }

    [Fact]
    public void GetProfilePicture_UserIsNull_ThrowArgumentNullException()
    {
      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        try
        {
          _ = unitOfWork.AttachmentRepository.GetProfilePicture(null);

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
    public void GetProfilePicture_UserHasNoProfilePicture_ReturnNull()
    {
      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        var data = unitOfWork.AttachmentRepository.GetProfilePicture(identityHelper.TestUser);

        Assert.Null(data);
      }
    }

    [Fact]
    public void GetProfilePicture_UserHasProfilePicture_ReturnData()
    {
      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        unitOfWork.AttachmentRepository.Add(testProfilePicture);

        unitOfWork.Complete();
      }

      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        var result = unitOfWork.AttachmentRepository.GetProfilePicture(identityHelper.TestUser);
        Assert.NotNull(result);

        var resultData = result.Value.Data;
        Assert.NotNull(resultData);
        Assert.Equal(data, resultData);

        var resultExtension = result.Value.Extension;
        Assert.NotNull(resultExtension);
        Assert.Equal(extension, resultExtension);
      }

      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        unitOfWork.AttachmentRepository.Remove(testProfilePicture);

        unitOfWork.Complete();
      }
    }

    [Fact]
    public void ChangeProfilePicture_UserIsNull_ThrowArgumentNullException()
    {
      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        try
        {
          var file = new Mock<IFormFile>().Object;
          unitOfWork.AttachmentRepository.ChangeProfilePicture(null, file);

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
    public void ChangeProfilePicture_FileIsNull_ThrowArgumentNullException()
    {
      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        try
        {
          unitOfWork.AttachmentRepository.ChangeProfilePicture(identityHelper.TestUser, null);

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
    public void ChangeProfilePicture_UserHasNoProfilePicture_NewEntryAddedToDatabase()
    {
      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        var profilePictures = unitOfWork.AttachmentRepository
          .Find(x => x.UserId == identityHelper.TestUser.Id && x.Type == AttachmentType.ProfilePicture)
          .ToList();

        Assert.Equal(0, profilePictures?.Count);
      }
      
      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {        
        var ms = new MemoryStream(data);
        var file = new FormFile(ms, 0, ms.Length, filename + extension, filename + extension);

        unitOfWork.AttachmentRepository.ChangeProfilePicture(identityHelper.TestUser, file);

        unitOfWork.Complete();        
      }

      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        var profilePictures = unitOfWork.AttachmentRepository
          .Find(x => x.UserId == identityHelper.TestUser.Id && x.Type == AttachmentType.ProfilePicture)
          .ToList();       

        Assert.Single(profilePictures);

        var profilePicture = profilePictures[0];
        Assert.Equal(filename, profilePicture.Filename);
        Assert.Equal(extension, profilePicture.Extension);
        Assert.Equal(data, profilePicture.Data);
      }

      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        var profilePicture = unitOfWork.AttachmentRepository
          .Find(x => x.UserId == identityHelper.TestUser.Id && x.Type == AttachmentType.ProfilePicture)
          .FirstOrDefault();
                
        unitOfWork.AttachmentRepository.Remove(profilePicture);
        unitOfWork.Complete();        
      }
    }

    [Fact]
    public void ChangeProfilePicture_UserHasProfilePicture_ProfilePictureIsOverridden()
    {
      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        var attachment = new Attachment
        {
          UserId = identityHelper.TestUser.Id,
          Type = AttachmentType.ProfilePicture,
          Filename = "wrongFile",
          Extension = ".txt",
          Data = new byte[] { 0x15, 0xa1, 0xbf, 0x69, 0x05, 0xd4, 0xca }
        };

        unitOfWork.AttachmentRepository.Add(attachment);
        unitOfWork.Complete();
      }

      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        var profilePictures = unitOfWork.AttachmentRepository
          .Find(x => x.UserId == identityHelper.TestUser.Id && x.Type == AttachmentType.ProfilePicture)
          .ToList();
        Assert.Single(profilePictures);
      }

      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        var ms = new MemoryStream(data);
        var file = new FormFile(ms, 0, ms.Length, filename + extension, filename + extension);

        unitOfWork.AttachmentRepository.ChangeProfilePicture(identityHelper.TestUser, file);

        unitOfWork.Complete();
      }

      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        var profilePictures = unitOfWork.AttachmentRepository
          .Find(x => x.UserId == identityHelper.TestUser.Id && x.Type == AttachmentType.ProfilePicture)
          .ToList();

        Assert.Single(profilePictures);

        var profilePicture = profilePictures[0];
        Assert.Equal(filename, profilePicture.Filename);
        Assert.Equal(extension, profilePicture.Extension);
        Assert.Equal(data, profilePicture.Data);
      }

      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        var profilePicture = unitOfWork.AttachmentRepository
          .Find(x => x.UserId == identityHelper.TestUser.Id && x.Type == AttachmentType.ProfilePicture)
          .FirstOrDefault();

        unitOfWork.AttachmentRepository.Remove(profilePicture);
        unitOfWork.Complete();        
      }
    }

    [Fact]
    public void DeleteProfilePicture_UserIsNull_ThrowArgumentNullException()
    {
      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        try
        {
          unitOfWork.AttachmentRepository.DeleteProfilePicture(null);

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
    public void DeleteProfilePicture_UserHasNoProfilePicture_ReturnFalse()
    {
      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        var result = unitOfWork.AttachmentRepository.DeleteProfilePicture(identityHelper.TestUser);
        unitOfWork.Complete();
        Assert.False(result);
      }
    }

    [Fact]
    public void DeleteProfilePicture_UserHasProfilePicture_ReturnTrue()
    {
      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        unitOfWork.AttachmentRepository.Add(testProfilePicture);

        unitOfWork.Complete();
      }
      
      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      { 
        var result = unitOfWork.AttachmentRepository.DeleteProfilePicture(identityHelper.TestUser);
        unitOfWork.Complete();
        Assert.True(result);
      }
      
      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        unitOfWork.AttachmentRepository.Remove(testProfilePicture);

        unitOfWork.Complete();
      }      
    }

    public void Dispose()
    {
      identityHelper.RemoveTestUser(databaseHelper.Context);
      databaseHelper.Dispose();
    }
  }
}