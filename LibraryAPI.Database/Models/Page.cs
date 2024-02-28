using System.Text.Json.Serialization;        

namespace LibraryAPI.Database.Models {
    public class Page
    {
        public int PageNumber { get; set; }
        public int BookId { get; set; }
        [JsonIgnore]
        public Book Book { get; set; }
        public string Content { get; set; }
    }
}