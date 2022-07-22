using System.ComponentModel.DataAnnotations;
using MyLand.Areas.Identity.Data;


namespace MyLand.Models
{
    public class Property
    {
        [Key]
        public int Id { get; set; }
        public PropertyType Type { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        [Display(Name = "Size (m2)")]
        public int Size { get; set; }
        public string Photo { get; set; }
        [Required]
        public System.DateTime Date { get; set; }
        public virtual MyLandUser User { get; set; }
        [Required]
        [Display(Name = "Activated")]
        public bool IsActive { get; set; }
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
