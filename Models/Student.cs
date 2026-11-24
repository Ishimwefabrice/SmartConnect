using System.ComponentModel.DataAnnotations;

namespace smartconnect.Models
{
    public class Student
    {
        [Required]
        public string StudentID { get; set; } = "";

        [Required]
        public string Name { get; set; } = "";

        [Required]
        [Range(1, 100)]
        public int Age { get; set; }

        [Required]
        public string Course { get; set; } = "";
    }
}
