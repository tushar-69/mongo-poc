namespace MongoPOC.Models.DTOs
{
    public class ProductDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }
        public List<ReviewDTO> Reviews { get; set; }
    }

    public class ReviewDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
    }
}
