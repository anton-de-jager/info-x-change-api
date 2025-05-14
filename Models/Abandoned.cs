namespace infoX.api.Models
{
    public class Abandoned
    {
        public int? companyID { get; set; }
        public DateTime? abandonedDate { get; set; }
        public string? idNumber { get; set; }
        public string? matterID { get; set; }
        public long? cellNumber { get; set; }
        public string? messageType { get; set; }
        public string? bookName { get; set; }
        public string? timeofLastMessage { get; set; }
    }

}
