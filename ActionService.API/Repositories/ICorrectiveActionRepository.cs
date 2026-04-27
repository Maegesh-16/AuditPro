using ActionService.API.Models;

namespace ActionService.API.Repositories
{
    public interface ICorrectiveActionRepository
    {
        Task<IEnumerable<CorrectiveAction>> GetAllAsync();
        Task<CorrectiveAction?> GetByIdAsync(int actionId);
        Task<IEnumerable<CorrectiveAction>> GetByObservationIdAsync(int observationId);
        Task<IEnumerable<CorrectiveAction>> GetByAssignedUserIdAsync(int userId);
        Task<CorrectiveAction> CreateAsync(CorrectiveAction action);
        Task UpdateAsync(CorrectiveAction action);
        Task SoftDeleteAsync(int actionId);
        Task AddHistoryAsync(ActionHistory history);
        Task<IEnumerable<ActionHistory>> GetHistoryByActionIdAsync(int actionId);
    }
}
