public class ProfessorRegistrationModel
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Password { get; set; }
    public string ProfilePictureUrl { get; set; }
    public int InstructionsCount { get; set; }
    public string[] Subjects { get; set; }
}