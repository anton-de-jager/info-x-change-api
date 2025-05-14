namespace infoX.api.Models
{
    public class AgentKPI
    {
        public string? agent { get; set; }
        public string? matterNumber { get; set; }
        public string? matterStatus { get; set; }
        public string? responseDuration { get; set; }
        public string? chatStatus { get; set; }
        public DateTime? chatDate { get; set; }
        public DateTime? agentResponseTime { get; set; }
        public string? fromClient { get; set; }
        public string? wrapUps { get; set; }
        public int? companyId { get; set; }
    }
}
