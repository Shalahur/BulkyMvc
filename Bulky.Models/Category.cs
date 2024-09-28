﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Models;

public class Category
{
    [Key] 
    public int Id { get; set; }
    
    [Required]
    [StringLength(Int32.MaxValue)]
    [DisplayName("Category Name")]
    public string Name { get; set; }

    [DisplayName("Display Order")]
    [Range(1, 100, ErrorMessage = "Display number must be between 1 and 100")]
    public int DisplayOrder { get; set; }
}