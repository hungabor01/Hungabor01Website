using Database.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace Database.UnitOfWork
{  
    public interface IUnitOfWork : IDisposable
    {
        public IAttachmentRepository AttachmentRepository { get; }
        public IAccountHistoryRepository AccountHistoryRepository { get; }
   
        public int Complete();

        public Task<int> CompleteAsync();
    }
}
