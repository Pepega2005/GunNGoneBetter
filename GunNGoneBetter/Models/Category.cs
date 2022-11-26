﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace GunNGoneBetter.Models
{
    public class Category
    {
        [Key]
        public int Id {get; set;}

        [Required]
        public string Name {get; set;}

        [Required]
        [DisplayName("Display Order")]
        [Range(1, int.MaxValue, ErrorMessage = "Value has to be higher than 0.")]
        public int DisplayOrder {get; set;}
    }
}
