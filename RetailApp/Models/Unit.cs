namespace RetailApp.Models
{
    public class Unit
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Piece, Box, Carton, etc.
        public string Abbreviation { get; set; } = string.Empty;
    }
}
