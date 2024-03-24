using System.ComponentModel.DataAnnotations;

namespace dotInstrukcije
{
    public class Professor
    {
        [Key]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public int InstructionsCount { get; set; }
        public string[] Subjects { get; set; }
    }
}
