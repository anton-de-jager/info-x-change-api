namespace infoX.api.Models
{
    public class AgentProductivity
    {
        public int? companyId { get; set; }
        public DateTime? date { get; set; }
        public string? conversationStart { get; set; }
        public string? agentName { get; set; }
        public int? qtyConversation { get; set; }
        public int? qtyMessages { get; set; }
        public string? book { get; set; }
        public string? debtorName { get; set; }
        public string? debtorID { get; set; }
    }
}
