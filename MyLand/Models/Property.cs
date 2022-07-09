using System.ComponentModel.DataAnnotations;
using MyLand.Areas.Identity.Data;


namespace MyLand.Models
{
    public class Property
    {
        [Key]
        public int Id { get; set; }
        public PropertyType Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int Size { get; set; }
        public string Photo { get; set; }
        public System.DateTime Date { get; set; }
        public string UserId { get; set; }
        public virtual MyLandUser User { get; set; }
        public int IsActive { get; set; }
    }
    
    public enum PropertyType
    {
        [Display(Name = "House")]
        House,
        [Display(Name = "Apartment")]
        Apartment,
        [Display(Name = "Plot of Land")]
        Land
    }
}
