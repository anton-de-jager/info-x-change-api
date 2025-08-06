namespace infoX.api.Models
{
    public class PlatformUser
    {
        public int? Id { get; set; }
        public int? PlatformId { get; set; }
        public int? UserId { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ChangedOn { get; set; }
        public int? ChangedBy { get; set; }
    }
}
