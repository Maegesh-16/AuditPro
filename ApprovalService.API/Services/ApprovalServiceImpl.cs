using ApprovalService.API.DTOs;
using ApprovalService.API.Enums;
using ApprovalService.API.Models;
using ApprovalService.API.Repositories;

namespace ApprovalService.API.Services
{
    public class ApprovalServiceImpl : IApprovalService
    {
        private readonly IApprovalRepository _repository;
        private readonly ILogger<ApprovalServiceImpl> _logger;

        // =======================================================================
        // TODO: Inject HttpClient or service clients for cross-service calls:
        //   - AuditService: to update audit status on approval/rejection
        //   - ObservationService: to update observation status on approval/rejection
        //   - ActionService: to update action status on closure approval/rejection
        //   - UserService: to validate RequestedByUserId and ActionByUserId
        //   - NotificationService: to notify requester of approval result
        // =======================================================================

        public ApprovalServiceImpl(
            IApprovalRepository repository,
            ILogger<ApprovalServiceImpl> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<ApprovalRequestResponseDto>> GetAllApprovalsAsync()
        {
            var approvals = await _repository.GetAllAsync();
            return approvals.Select(MapToResponseDto);
        }

        public async Task<ApprovalRequestResponseDto?> GetApprovalByIdAsync(int approvalId)
        {
            var approval = await _repository.GetByIdAsync(approvalId);
            return approval == null ? null : MapToResponseDto(approval);
        }

        public async Task<IEnumerable<ApprovalRequestResponseDto>> GetApprovalsByEntityAsync(EntityType entityType, int entityId)
        {
            var approvals = await _repository.GetByEntityAsync(entityType, entityId);
            return approvals.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<ApprovalRequestResponseDto>> GetPendingApprovalsAsync()
        {
            var approvals = await _repository.GetByStatusAsync(ApprovalStatus.Pending);
            return approvals.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<ApprovalRequestResponseDto>> GetApprovalsByRequesterAsync(int userId)
        {
            var approvals = await _repository.GetByRequestedByUserIdAsync(userId);
            return approvals.Select(MapToResponseDto);
        }

        public async Task<ApprovalRequestResponseDto> CreateApprovalRequestAsync(CreateApprovalRequestDto dto)
        {
            // =======================================================================
            // DUMMY: Validate RequestedByUserId exists via UserService API
            // var user = await _userServiceClient.GetUserAsync(dto.RequestedByUserId);
            // if (user == null) throw new ArgumentException("User not found");
            _logger.LogWarning("[DUMMY] Skipping RequestedByUserId validation for UserId: {UserId}. " +
                "Replace with actual UserService API call.", dto.RequestedByUserId);
            // =======================================================================

            // =======================================================================
            // DUMMY: Validate EntityId exists via respective service API
            // Based on dto.EntityType:
            //   Audit -> AuditService.GetAuditAsync(dto.EntityId)
            //   Observation -> ObservationService.GetObservationAsync(dto.EntityId)
            //   Action -> ActionService.GetActionAsync(dto.EntityId)
            _logger.LogWarning("[DUMMY] Skipping EntityId validation for EntityType: {EntityType}, EntityId: {EntityId}. " +
                "Replace with actual service API call.", dto.EntityType, dto.EntityId);
            // =======================================================================

            var request = new ApprovalRequest
            {
                EntityType = dto.EntityType,
                EntityId = dto.EntityId,
                ApprovalType = dto.ApprovalType,
                RequestedByUserId = dto.RequestedByUserId,
                Status = ApprovalStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(request);

            // =======================================================================
            // DUMMY: Notify approver(s) via NotificationService API
            // await _notificationServiceClient.SendNotificationAsync(new {
            //     UserId = approverUserId,  // determined by role/approval type
            //     Message = $"New approval request (ID: {created.ApprovalId}) for {dto.EntityType} requires your review."
            // });
            _logger.LogWarning("[DUMMY] Skipping notification for new approval request. " +
                "Replace with actual NotificationService API call.");
            // =======================================================================

            return MapToResponseDto(created);
        }

        public async Task<ApprovalRequestResponseDto?> ProcessApprovalAsync(int approvalId, ProcessApprovalDto dto)
        {
            var approval = await _repository.GetByIdAsync(approvalId);
            if (approval == null) return null;

            if (approval.Status != ApprovalStatus.Pending)
            {
                throw new InvalidOperationException("Only pending approvals can be processed.");
            }

            // =======================================================================
            // DUMMY: Validate ActionByUserId exists and has approver role via UserService API
            // var user = await _userServiceClient.GetUserAsync(dto.ActionByUserId);
            // if (user == null || !user.HasApproverRole) throw new UnauthorizedAccessException("Not authorized");
            _logger.LogWarning("[DUMMY] Skipping ActionByUserId validation for UserId: {UserId}. " +
                "Replace with actual UserService API call.", dto.ActionByUserId);
            // =======================================================================

            // Update approval status
            approval.Status = dto.Action == ApprovalAction.Approved
                ? ApprovalStatus.Approved
                : ApprovalStatus.Rejected;
            await _repository.UpdateAsync(approval);

            // Record history
            var history = new ApprovalHistory
            {
                ApprovalId = approvalId,
                ActionByUserId = dto.ActionByUserId,
                Action = dto.Action,
                Comments = dto.Comments,
                ActionDate = DateTime.UtcNow
            };
            await _repository.AddHistoryAsync(history);

            // =======================================================================
            // DUMMY: Callback to originating service based on EntityType and ApprovalType
            // This is where the approval result triggers status changes in other services:
            //
            // AuditCreation approval:
            //   Approved -> AuditService: Audit status Draft -> Scheduled
            //   Rejected -> AuditService: Audit stays Draft (with comments)
            //
            // AuditFindings approval:
            //   Approved -> AuditService: Audit status -> FindingsApproved
            //   Rejected -> ObservationService: Observations sent back to Auditor
            //
            // CorrectiveActionClosure approval:
            //   Approved -> ActionService: Action status -> Closed
            //   Rejected -> ActionService: Action status -> Reopened
            //
            // FinalAuditClosure approval:
            //   Approved -> AuditService: Audit status -> Completed (locked)
            //   Rejected -> AuditService: Audit remains open
            //
            _logger.LogWarning("[DUMMY] Skipping callback to {EntityType} service for ApprovalId: {ApprovalId}, " +
                "Result: {Result}. Replace with actual service API call.",
                approval.EntityType, approvalId, dto.Action);
            // =======================================================================

            // =======================================================================
            // DUMMY: Notify requester of approval result via NotificationService API
            // await _notificationServiceClient.SendNotificationAsync(new {
            //     UserId = approval.RequestedByUserId,
            //     Message = $"Your approval request (ID: {approvalId}) has been {dto.Action}."
            // });
            _logger.LogWarning("[DUMMY] Skipping notification for approval result. " +
                "Replace with actual NotificationService API call.");
            // =======================================================================

            return MapToResponseDto(approval);
        }

        public async Task<IEnumerable<ApprovalHistoryResponseDto>> GetApprovalHistoryAsync(int approvalId)
        {
            var histories = await _repository.GetHistoryByApprovalIdAsync(approvalId);
            return histories.Select(h => new ApprovalHistoryResponseDto
            {
                HistoryId = h.HistoryId,
                ApprovalId = h.ApprovalId,
                ActionByUserId = h.ActionByUserId,
                Action = h.Action,
                Comments = h.Comments,
                ActionDate = h.ActionDate
            });
        }

        private static ApprovalRequestResponseDto MapToResponseDto(ApprovalRequest request)
        {
            return new ApprovalRequestResponseDto
            {
                ApprovalId = request.ApprovalId,
                EntityType = request.EntityType,
                EntityId = request.EntityId,
                ApprovalType = request.ApprovalType,
                RequestedByUserId = request.RequestedByUserId,
                Status = request.Status,
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt
            };
        }
    }
}
