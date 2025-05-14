namespace infoX.api.Models
{
    public class DebitOrderArrangements
    {
        public int? companyId { get; set; }
        public DateTime? arrangementDate { get; set; }
        public string? idNumber { get; set; }
        public string? matterID { get; set; }
        public string? bookName { get; set; }
        public string? bankInstitution { get; set; }
        public long? bankAccount { get; set; }
        public string? bankAccHolder { get; set; }
        public double? firstPayAmount { get; set; }
        public DateTime? firstPayDate { get; set; }
        public string? payFrequency { get; set; }
        public string? installmentCounts { get; set; }
    }
}
