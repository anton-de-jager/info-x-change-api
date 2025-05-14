namespace infoX.api.Models
{
    public class Arrangements
    {
        public int? companyId { get; set; }
        public DateTime? arrangementDate { get; set; }
        public string? arrangementTime { get; set; }
        public string? idNumber { get; set; }
        public string? matterId { get; set; }
        public string? cellNumber { get; set; }
        public string? messageType { get; set; }
        public string? typeOfArrangement { get; set; }
        public string? bookName { get; set; }
        public double? firstPayAmount { get; set; }
        public DateTime? firstPayDate { get; set; }
        public string? payFrequency { get; set; }
        public string? installmentCounts { get; set; }
    }
}
