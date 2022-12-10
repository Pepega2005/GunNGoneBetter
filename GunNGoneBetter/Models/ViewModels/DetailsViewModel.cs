namespace GunNGoneBetter.Models.ViewModels
{
    public class DetailsViewModel
    {
        public Product Product { get; set; }
        public bool isInCart { get; set; }

        // может быть конструктор и инициализация данных
        public DetailsViewModel()
        {
            Product = new Product();
        }
    }
}
