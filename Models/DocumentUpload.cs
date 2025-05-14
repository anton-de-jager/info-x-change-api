namespace infoX.api.Models
{
    public class DocumentUpload
    {
        public int? companyID { get; set; }
        public DateTime? date { get; set; }
        public string? time { get; set; }
        public string? matter { get; set; }
        public string? book { get; set; }
        public string? number { get; set; }
        public string? document { get; set; }
        public string? documentUrl { get; set; }
        public string? documentType { get; set; }
        public string? path { get; set; }
    }
}
