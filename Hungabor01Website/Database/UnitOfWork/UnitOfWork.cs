using Database.Core;
using Database.Repositories.Interfaces;
using System.Threading.Tasks;

namespace Database.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IAttachmentRepository AttachmentRepository { get; }
        public IAccountHistoryRepository AccountHistoryRepository { get; }

        public UnitOfWork(
            AppDbContext context,
            IAttachmentRepository attachments,
            IAccountHistoryRepository accountHistories)
        {
            _context = context;
            AttachmentRepository = attachments;
            AccountHistoryRepository = accountHistories;

            AttachmentRepository.Initialize(_context);
            AccountHistoryRepository.Initialize(_context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
