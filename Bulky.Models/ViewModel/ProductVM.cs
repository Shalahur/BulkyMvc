using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bulky.Models.ViewModel;

public class ProductVM
{
    public Product Product { get; set; }
    public IEnumerable<SelectListItem> Categories { get; set; }
    
}