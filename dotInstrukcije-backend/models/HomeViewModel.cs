namespace dotInstrukcije.Models
{
    public class HomeViewModel
    {
        public List<Professor> Professors { get; set; } = new List<Professor>();
        public List<Student> Students { get; set; } = new List<Student>();
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public List<InstructionsDate> InstructionsDates { get; set; } = new List<InstructionsDate>();
    }
}
