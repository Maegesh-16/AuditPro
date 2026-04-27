using ActionService.API.Enums;

namespace ActionService.API.DTOs
{
    public class CreateCorrectiveActionDto
    {
        public int ObservationId { get; set; }
        public int AssignedToUserId { get; set; }
        public string ActionDescription { get; set; } = string.Empty;
        public string RootCause { get; set; } = string.Empty;
        public string ExpectedOutcome { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
    }
}
