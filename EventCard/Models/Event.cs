namespace EventCard.Models
{
    public class Event
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Location { get; set; } = string.Empty;
    }
}