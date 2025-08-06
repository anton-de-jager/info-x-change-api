namespace infoX.api.Models
{
    public class Department
    {
        public int? Id { get; set; }
        public string? Description { get; set; }
        public int? CompanyId { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ChangedOn { get; set; }
        public int? ChangedBy { get; set; }
    }
}
