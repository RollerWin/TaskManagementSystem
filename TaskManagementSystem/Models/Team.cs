using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class Team
    {
        [Key]
        public int TeamID { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        // Навигационное свойство для связи с пользователями
        public ICollection<User> Users { get; set; }
    }
}
