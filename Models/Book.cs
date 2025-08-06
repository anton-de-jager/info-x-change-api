namespace infoX.api.Models
{
    public class Book
    {
        public int? Id { get; set; }
        public int? DepartmentId { get; set; }
        public string? Description { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ChangedOn { get; set; }
        public int? ChangedBy { get; set; }
    }
}