namespace ActionService.API.DTOs
{
    public class ActionHistoryResponseDto
    {
        public int Id { get; set; }
        public int ActionId { get; set; }
        public string FieldChanged { get; set; } = string.Empty;
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
    }
}
