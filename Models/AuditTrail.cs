namespace infoX.api.Models
{
    public class AuditTrail
    {
        public int? Id { get; set; }

        public string? TableName { get; set; }

        public string? Action { get; set; }

        public string? PrimaryKey { get; set; }

        public string? ChangedColumns { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        public string? ChangedBy { get; set; }

        public DateTime? ChangedOn { get; set; }
    }

}
