namespace infoX.api.Models
{
    public class Payment
    {
        public int? companyId { get; set; }
        public DateTime? createdAt { get; set; }
        public string? name { get; set; }
        public string? surname { get; set; }
        public string? idNumber { get; set; }
        public string? bookName { get; set; }
        public string? amount { get; set; }
        public string? status { get; set; }
        public string? responseStatus { get; set; }
    }

}
