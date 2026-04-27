using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActionService.API.Models
{
    public class ActionHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ActionId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FieldChanged { get; set; } = string.Empty;

        [MaxLength(500)]
        public string OldValue { get; set; } = string.Empty;

        [MaxLength(500)]
        public string NewValue { get; set; } = string.Empty;

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey(nameof(ActionId))]
        public CorrectiveAction CorrectiveAction { get; set; } = null!;
    }
}
