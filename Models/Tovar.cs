namespace WebApplicationDB1.Models
{
    public class Tovar
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
        public double Price { get; set; }
        public long Count { get; set; }
        public byte[] Url { get; set; }
        /*public string Url { get; set; }*/
    }
}
