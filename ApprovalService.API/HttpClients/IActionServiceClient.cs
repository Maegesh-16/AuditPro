namespace ApprovalService.API.HttpClients
{
    public class UpdateActionStatusPayload
    {
        public int Status { get; set; } // 3 = Closed, 4 = Reopened
    }

    public interface IActionServiceClient
    {
        Task UpdateActionStatusAsync(int actionId, UpdateActionStatusPayload payload);
    }
}
