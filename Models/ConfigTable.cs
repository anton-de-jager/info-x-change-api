namespace infoX.api.Models
{
    public class ConfigTable
    {
        public int Id { get; set; }

        public string? Property { get; set; }

        public string? Label { get; set; }

        public bool? AllowExportExcel { get; set; }

        public bool? AllowExportPdf { get; set; }

        public bool? AllowFilter { get; set; }

        public bool? SetPageName { get; set; }

        public bool? AllowAdd { get; set; }

        public bool? AllowEdit { get; set; }

        public bool? AllowDelete { get; set; }
        public int? ConfigTableId { get; set; }
        public string? DatabaseName { get; set; }
        
        public bool? Active { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ChangedOn { get; set; }
        public int? ChangedBy { get; set; }
    }
}
