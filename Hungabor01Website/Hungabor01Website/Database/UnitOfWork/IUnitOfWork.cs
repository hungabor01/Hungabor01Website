using Hungabor01Website.Database.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace Hungabor01Website.Database.UnitOfWork
{  
    public interface IUnitOfWork : IDisposable
    {
        public IAttachmentRepository AttachmentRepository { get; }
        public IAccountHistoryRepository AccountHistoryRepository { get; }
   
        public int Complete();

        public Task<int> CompleteAsync();
    }
}
