namespace dotInstrukcije
{ 
	public class Subject
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Url { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public List<Professor> professors { get; set; }
	}
}
