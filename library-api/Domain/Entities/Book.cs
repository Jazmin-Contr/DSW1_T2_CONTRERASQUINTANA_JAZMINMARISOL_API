namespace library_api.Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();

        public bool HasStock()
        {
            return Stock > 0;
        }

        public void DecreaseStock()
        {
            if (Stock > 0)
                Stock--;
        }

        public void IncreaseStock()
        {
            Stock++;
        }
    }
}