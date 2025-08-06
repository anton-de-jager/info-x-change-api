namespace infoX.api.Models
{
    public class Company
    {
        public int? Id { get; set; }

        public string? Description { get; set; }

        public string? ClientSupportTelephone { get; set; }

        public string? ClientSupportEmail { get; set; }

        public string? RegistrationNo { get; set; }

        public string? Vat { get; set; }

        public string? PhysicalAddress { get; set; }

        public string? PhoneNumberId { get; set; }

        public string? WabaId { get; set; }

        public string? BusinessId { get; set; }

        public string? AccessToken { get; set; }

        public bool? Active { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ChangedOn { get; set; }

        public int? ChangedBy { get; set; }

        public ICollection<Message> Messages { get; set; }
    }
}