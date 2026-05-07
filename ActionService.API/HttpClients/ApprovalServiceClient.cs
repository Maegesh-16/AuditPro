using System.Text;
using System.Text.Json;

namespace ActionService.API.HttpClients
{
    public class ApprovalServiceClient : IApprovalServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApprovalServiceClient> _logger;

        public ApprovalServiceClient(HttpClient httpClient, ILogger<ApprovalServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task CreateApprovalRequestAsync(CreateApprovalRequestPayload payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/approvals", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("ApprovalService returned {StatusCode} when creating approval request for ActionId: {ActionId}",
                    response.StatusCode, payload.EntityId);
                throw new HttpRequestException($"ApprovalService call failed with status {response.StatusCode}");
            }
        }
    }
}
