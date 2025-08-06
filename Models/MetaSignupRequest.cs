namespace infoX.api.Models
{
    public class MetaSignupRequest
    {
        public string Code { get; set; }
        public int? CompanyId { get; set; }
        public int? RoleId { get; set; }
        public string? Title { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? MaidenName { get; set; }
        public string? KnownName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? IdNumber { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }    // optional: profile picture
        public bool? Active { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ChangedOn { get; set; }
        public int? ChangedBy { get; set; }
    }
}
