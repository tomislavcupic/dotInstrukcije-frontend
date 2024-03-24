using System.ComponentModel.DataAnnotations;

public class StudentRegistrationModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Surname is required")]
    public string Surname { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Profile picture URL is required")]
    public IFormFile ProfilePictureUrl { get; set; }
}