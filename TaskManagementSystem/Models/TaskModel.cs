using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class TaskModel
    {
        [Key]
        public int TaskID { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime DueDate { get; set; }

        // Внешние ключи
        public int AssigneeID { get; set; }
        public User Assignee { get; set; }

        public int ProjectID { get; set; }
        public Project Project { get; set; }
    }
}
