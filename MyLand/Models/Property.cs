using System.ComponentModel.DataAnnotations;
using MyLand.Areas.Identity.Data;


namespace MyLand.Models
{
    public class Property
    {
        [Key]
        public int Id { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int Size { get; set; }
        public string Photo { get; set; }
        public string Date { get; set; }
        public string UserId { get; set; }
        public virtual MyLandUser User { get; set; }
        public int Active { get; set; }
    }
}
