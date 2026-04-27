using Microsoft.EntityFrameworkCore;
using ActionService.API.Data;
using ActionService.API.Models;

namespace ActionService.API.Repositories
{
    public class CorrectiveActionRepository : ICorrectiveActionRepository
    {
        private readonly ActionDbContext _context;

        public CorrectiveActionRepository(ActionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CorrectiveAction>> GetAllAsync()
        {
            return await _context.CorrectiveActions
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<CorrectiveAction?> GetByIdAsync(int actionId)
        {
            return await _context.CorrectiveActions
                .FirstOrDefaultAsync(a => a.ActionId == actionId);
        }

        public async Task<IEnumerable<CorrectiveAction>> GetByObservationIdAsync(int observationId)
        {
            return await _context.CorrectiveActions
                .Where(a => a.ObservationId == observationId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CorrectiveAction>> GetByAssignedUserIdAsync(int userId)
        {
            return await _context.CorrectiveActions
                .Where(a => a.AssignedToUserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<CorrectiveAction> CreateAsync(CorrectiveAction action)
        {
            _context.CorrectiveActions.Add(action);
            await _context.SaveChangesAsync();
            return action;
        }

        public async Task UpdateAsync(CorrectiveAction action)
        {
            action.UpdatedAt = DateTime.UtcNow;
            _context.CorrectiveActions.Update(action);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int actionId)
        {
            var action = await _context.CorrectiveActions
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(a => a.ActionId == actionId);

            if (action != null)
            {
                action.IsDeleted = true;
                action.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddHistoryAsync(ActionHistory history)
        {
            _context.ActionHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ActionHistory>> GetHistoryByActionIdAsync(int actionId)
        {
            return await _context.ActionHistories
                .Where(h => h.ActionId == actionId)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();
        }
    }
}
