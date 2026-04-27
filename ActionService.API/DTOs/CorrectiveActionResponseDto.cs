using ActionService.API.Enums;

namespace ActionService.API.DTOs
{
    public class CorrectiveActionResponseDto
    {
        public int ActionId { get; set; }
        public int ObservationId { get; set; }
        public int AssignedToUserId { get; set; }
        public string ActionDescription { get; set; } = string.Empty;
        public string RootCause { get; set; } = string.Empty;
        public string ExpectedOutcome { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public ActionStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
