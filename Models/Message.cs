namespace infoX.api.Models
{
    public enum Direction { Outbound, Inbound }

    public class Message
    {
        public int? Id { get; set; }
        public int? CompanyId { get; set; }
        public Company? Company { get; set; }

        public Direction? Direction { get; set; }
        public string? Content { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
