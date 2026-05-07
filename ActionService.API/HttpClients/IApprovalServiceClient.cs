namespace ActionService.API.HttpClients
{
    public class CreateApprovalRequestPayload
    {
        public int EntityType { get; set; }   // 2 = Action
        public int EntityId { get; set; }
        public int ApprovalType { get; set; } // 2 = CorrectiveActionClosure
        public int RequestedByUserId { get; set; }
    }

    public interface IApprovalServiceClient
    {
        Task CreateApprovalRequestAsync(CreateApprovalRequestPayload payload);
    }
}
