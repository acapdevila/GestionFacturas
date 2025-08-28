namespace GestionFacturas.Web.Framework.Grid
{
    public class GridParams
    {
        public string Sort { get; set; } = string.Empty;
        public string Order { get; set; } = string.Empty;
        public string Search { get; set; } = string.Empty;

        //public string CustomSearch { get; set; } = string.Empty;
        public int Limit { get; set; }
        public int Offset { get; set; }
        public virtual IEnumerable<string> SearchColumns { get; } = [];
        public bool IsSearch() => !string.IsNullOrEmpty(Search);

    }
}