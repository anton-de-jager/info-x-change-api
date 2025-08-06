namespace infoX.api.Models
{
    public class ConfigColumn
    {
        public int Id { get; set; }

        public int? ConfigTableId { get; set; }

        public string? Property { get; set; }

        public string? Label { get; set; }

        public string? DataType { get; set; }

        public string? FilterType { get; set; }

        public bool? VisibleTable { get; set; }

        public bool? VisibleDialog { get; set; }

        public bool? VisibleFilter { get; set; }

        public int? Index { get; set; }

        public bool? AllowUpdate { get; set; }
        public bool? Required { get; set; }

        public bool? IsForeignKey { get; set; }
        public bool? IncludeInsert { get; set; }
        public bool? IncludeUpdate { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ChangedOn { get; set; }
        public int? ChangedBy { get; set; }
    }
}
