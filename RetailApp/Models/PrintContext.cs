namespace RetailApp.Models
{
    public class PrintContext
    {
        public object Invoice { get; set; }
        public PrintTemplate Template { get; set; }
        public AppSettings GlobalSettings { get; set; }
    }
}
