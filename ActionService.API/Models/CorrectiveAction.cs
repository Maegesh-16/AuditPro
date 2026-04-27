using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ActionService.API.Enums;

namespace ActionService.API.Models
{
    public class CorrectiveAction
    {
        [Key]
        public int ActionId { get; set; }

        [Required]
        public int ObservationId { get; set; }

        [Required]
        public int AssignedToUserId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string ActionDescription { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string RootCause { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string ExpectedOutcome { get; set; } = string.Empty;

        [Required]
        public DateOnly DueDate { get; set; }

        [Required]
        public ActionStatus Status { get; set; } = ActionStatus.Open;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation property
        public ICollection<ActionHistory> ActionHistories { get; set; } = new List<ActionHistory>();
    }
}
