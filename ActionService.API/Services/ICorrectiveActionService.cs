using ActionService.API.DTOs;

namespace ActionService.API.Services
{
    public interface ICorrectiveActionService
    {
        Task<IEnumerable<CorrectiveActionResponseDto>> GetAllActionsAsync();
        Task<CorrectiveActionResponseDto?> GetActionByIdAsync(int actionId);
        Task<IEnumerable<CorrectiveActionResponseDto>> GetActionsByObservationIdAsync(int observationId);
        Task<IEnumerable<CorrectiveActionResponseDto>> GetActionsByAssignedUserIdAsync(int userId);
        Task<CorrectiveActionResponseDto> CreateActionAsync(CreateCorrectiveActionDto dto);
        Task<CorrectiveActionResponseDto?> UpdateActionAsync(int actionId, UpdateCorrectiveActionDto dto);
        Task<bool> RequestClosureAsync(int actionId);
        Task<bool> SoftDeleteAsync(int actionId);
        Task<IEnumerable<ActionHistoryResponseDto>> GetActionHistoryAsync(int actionId);
    }
}
