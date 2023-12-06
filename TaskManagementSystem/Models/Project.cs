using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class Project
    {
        [Key]
        public int ProjectID { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        // Внешний ключ
        public int TeamID { get; set; }
        public Team Team { get; set; }
    }
}
