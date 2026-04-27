using ActionService.API.DTOs;
using ActionService.API.Enums;
using ActionService.API.Models;
using ActionService.API.Repositories;

namespace ActionService.API.Services
{
    public class CorrectiveActionService : ICorrectiveActionService
    {
        private readonly ICorrectiveActionRepository _repository;
        private readonly ILogger<CorrectiveActionService> _logger;

        // =======================================================================
        // TODO: Inject HttpClient or service clients for cross-service calls:
        //   - ObservationService: to validate ObservationId exists
        //   - UserService: to validate AssignedToUserId exists
        //   - ApprovalService: to create approval request on closure
        //   - NotificationService: to notify assigned user
        // =======================================================================

        public CorrectiveActionService(
            ICorrectiveActionRepository repository,
            ILogger<CorrectiveActionService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<CorrectiveActionResponseDto>> GetAllActionsAsync()
        {
            var actions = await _repository.GetAllAsync();
            return actions.Select(MapToResponseDto);
        }

        public async Task<CorrectiveActionResponseDto?> GetActionByIdAsync(int actionId)
        {
            var action = await _repository.GetByIdAsync(actionId);
            return action == null ? null : MapToResponseDto(action);
        }

        public async Task<IEnumerable<CorrectiveActionResponseDto>> GetActionsByObservationIdAsync(int observationId)
        {
            var actions = await _repository.GetByObservationIdAsync(observationId);
            return actions.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<CorrectiveActionResponseDto>> GetActionsByAssignedUserIdAsync(int userId)
        {
            var actions = await _repository.GetByAssignedUserIdAsync(userId);
            return actions.Select(MapToResponseDto);
        }

        public async Task<CorrectiveActionResponseDto> CreateActionAsync(CreateCorrectiveActionDto dto)
        {
            // =======================================================================
            // DUMMY: Validate ObservationId exists via ObservationService API
            // var observation = await _observationServiceClient.GetObservationAsync(dto.ObservationId);
            // if (observation == null) throw new ArgumentException("Observation not found");
            _logger.LogWarning("[DUMMY] Skipping ObservationId validation for ObservationId: {ObservationId}. " +
                "Replace with actual ObservationService API call.", dto.ObservationId);
            // =======================================================================

            // =======================================================================
            // DUMMY: Validate AssignedToUserId exists via UserService API
            // var user = await _userServiceClient.GetUserAsync(dto.AssignedToUserId);
            // if (user == null) throw new ArgumentException("User not found");
            _logger.LogWarning("[DUMMY] Skipping AssignedToUserId validation for UserId: {UserId}. " +
                "Replace with actual UserService API call.", dto.AssignedToUserId);
            // =======================================================================

            var action = new CorrectiveAction
            {
                ObservationId = dto.ObservationId,
                AssignedToUserId = dto.AssignedToUserId,
                ActionDescription = dto.ActionDescription,
                RootCause = dto.RootCause,
                ExpectedOutcome = dto.ExpectedOutcome,
                DueDate = dto.DueDate,
                Status = ActionStatus.Open,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(action);

            // =======================================================================
            // DUMMY: Notify assigned user via NotificationService API
            // await _notificationServiceClient.SendNotificationAsync(new {
            //     UserId = dto.AssignedToUserId,
            //     Message = $"You have been assigned a corrective action (ID: {created.ActionId})"
            // });
            _logger.LogWarning("[DUMMY] Skipping notification for ActionAssigned event. " +
                "Replace with actual NotificationService API call.");
            // =======================================================================

            return MapToResponseDto(created);
        }

        public async Task<CorrectiveActionResponseDto?> UpdateActionAsync(int actionId, UpdateCorrectiveActionDto dto)
        {
            var action = await _repository.GetByIdAsync(actionId);
            if (action == null) return null;

            if (action.Status == ActionStatus.Closed)
            {
                throw new InvalidOperationException("Cannot update a closed corrective action.");
            }

            // Track changes for history
            if (dto.ActionDescription != null && dto.ActionDescription != action.ActionDescription)
            {
                await TrackChange(actionId, nameof(action.ActionDescription), action.ActionDescription, dto.ActionDescription);
                action.ActionDescription = dto.ActionDescription;
            }

            if (dto.RootCause != null && dto.RootCause != action.RootCause)
            {
                await TrackChange(actionId, nameof(action.RootCause), action.RootCause, dto.RootCause);
                action.RootCause = dto.RootCause;
            }

            if (dto.ExpectedOutcome != null && dto.ExpectedOutcome != action.ExpectedOutcome)
            {
                await TrackChange(actionId, nameof(action.ExpectedOutcome), action.ExpectedOutcome, dto.ExpectedOutcome);
                action.ExpectedOutcome = dto.ExpectedOutcome;
            }

            if (dto.DueDate.HasValue && dto.DueDate.Value != action.DueDate)
            {
                await TrackChange(actionId, nameof(action.DueDate), action.DueDate.ToString(), dto.DueDate.Value.ToString());
                action.DueDate = dto.DueDate.Value;
            }

            if (dto.Status.HasValue && dto.Status.Value != action.Status)
            {
                ValidateStatusTransition(action.Status, dto.Status.Value);
                await TrackChange(actionId, nameof(action.Status), action.Status.ToString(), dto.Status.Value.ToString());
                action.Status = dto.Status.Value;
            }

            await _repository.UpdateAsync(action);
            return MapToResponseDto(action);
        }

        public async Task<bool> RequestClosureAsync(int actionId)
        {
            var action = await _repository.GetByIdAsync(actionId);
            if (action == null) return false;

            if (action.Status != ActionStatus.InProgress)
            {
                throw new InvalidOperationException("Only actions with 'InProgress' status can request closure.");
            }

            await TrackChange(actionId, nameof(action.Status), action.Status.ToString(), ActionStatus.ClosureRequested.ToString());
            action.Status = ActionStatus.ClosureRequested;
            await _repository.UpdateAsync(action);

            // =======================================================================
            // DUMMY: Create approval request via ApprovalService API
            // await _approvalServiceClient.CreateApprovalRequestAsync(new {
            //     EntityType = "Action",
            //     EntityId = actionId,
            //     ApprovalType = "ActionClosure",
            //     RequestedByUserId = action.AssignedToUserId
            // });
            _logger.LogWarning("[DUMMY] Skipping ApprovalService call for closure request of ActionId: {ActionId}. " +
                "Replace with actual ApprovalService API call.", actionId);
            // =======================================================================

            // =======================================================================
            // DUMMY: Notify approver via NotificationService API
            // await _notificationServiceClient.SendNotificationAsync(new {
            //     UserId = approverUserId,   // get from UserService based on role/dept
            //     Message = $"Corrective action (ID: {actionId}) closure requested. Please review."
            // });
            _logger.LogWarning("[DUMMY] Skipping notification for closure request. " +
                "Replace with actual NotificationService API call.");
            // =======================================================================

            return true;
        }

        public async Task<bool> SoftDeleteAsync(int actionId)
        {
            var action = await _repository.GetByIdAsync(actionId);
            if (action == null) return false;

            await _repository.SoftDeleteAsync(actionId);
            return true;
        }

        public async Task<IEnumerable<ActionHistoryResponseDto>> GetActionHistoryAsync(int actionId)
        {
            var histories = await _repository.GetHistoryByActionIdAsync(actionId);
            return histories.Select(h => new ActionHistoryResponseDto
            {
                Id = h.Id,
                ActionId = h.ActionId,
                FieldChanged = h.FieldChanged,
                OldValue = h.OldValue,
                NewValue = h.NewValue,
                ChangedAt = h.ChangedAt
            });
        }

        private async Task TrackChange(int actionId, string field, string oldValue, string newValue)
        {
            var history = new ActionHistory
            {
                ActionId = actionId,
                FieldChanged = field,
                OldValue = oldValue,
                NewValue = newValue,
                ChangedAt = DateTime.UtcNow
            };
            await _repository.AddHistoryAsync(history);
        }

        private static void ValidateStatusTransition(ActionStatus current, ActionStatus next)
        {
            var validTransitions = new Dictionary<ActionStatus, ActionStatus[]>
            {
                { ActionStatus.Open, new[] { ActionStatus.InProgress } },
                { ActionStatus.InProgress, new[] { ActionStatus.ClosureRequested } },
                { ActionStatus.ClosureRequested, new[] { ActionStatus.Closed, ActionStatus.Reopened } },
                { ActionStatus.Reopened, new[] { ActionStatus.InProgress } }
            };

            if (!validTransitions.ContainsKey(current) || !validTransitions[current].Contains(next))
            {
                throw new InvalidOperationException(
                    $"Invalid status transition from '{current}' to '{next}'.");
            }
        }

        private static CorrectiveActionResponseDto MapToResponseDto(CorrectiveAction action)
        {
            return new CorrectiveActionResponseDto
            {
                ActionId = action.ActionId,
                ObservationId = action.ObservationId,
                AssignedToUserId = action.AssignedToUserId,
                ActionDescription = action.ActionDescription,
                RootCause = action.RootCause,
                ExpectedOutcome = action.ExpectedOutcome,
                DueDate = action.DueDate,
                Status = action.Status,
                CreatedAt = action.CreatedAt,
                UpdatedAt = action.UpdatedAt
            };
        }
    }
}
