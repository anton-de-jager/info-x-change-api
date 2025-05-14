namespace infoX.api.Models
{
    public class ConfigTable
    {
        public Guid Id { get; set; }

        public string? Property { get; set; }

        public string? Label { get; set; }

        public bool? AllowExportExcel { get; set; }

        public bool? AllowExportPdf { get; set; }

        public bool? AllowFilter { get; set; }
    }
}
