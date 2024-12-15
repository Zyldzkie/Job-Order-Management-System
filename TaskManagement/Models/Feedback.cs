namespace TaskManagement.Models
{
    public class Feedback
    {
        public int FeedBackId { get; set; }
        public int ProjectID { get; set; }
        public string Username { get; set; }
        public string Comments { get; set; }
        public int Rating { get; set; }
        public DateTime SubmittedOn { get; set; }

        public Project Project { get; set; }
    }
}
