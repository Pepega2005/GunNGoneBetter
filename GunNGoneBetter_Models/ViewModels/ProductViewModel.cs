﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace GunNGoneBetter_Models.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }

        public IEnumerable<SelectListItem> CategoriesList { get; set; }

        public IEnumerable<SelectListItem> MyModelsList { get; set; }
    }
}
