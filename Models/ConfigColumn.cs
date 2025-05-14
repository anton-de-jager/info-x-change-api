namespace infoX.api.Models
{
    public class ConfigColumn
    {
        public Guid Id { get; set; }

        public Guid? ConfigTableId { get; set; }

        public string? Property { get; set; }

        public string? Label { get; set; }

        public string? DataType { get; set; }

        public string? FilterType { get; set; }

        public bool? VisibleTable { get; set; }

        public bool? VisibleDialog { get; set; }

        public bool? VisibleFilter { get; set; }

        public int? Index { get; set; }
    }
}
