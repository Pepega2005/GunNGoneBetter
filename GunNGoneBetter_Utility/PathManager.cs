﻿namespace GunNGoneBetter_Utility
{
    public class PathManager
    {
        public const string ImageProductPath = @"\images\product\";
        public const string SessionCart = "SessionCart";
        public const string SessionQuery = "SessionQuery";

        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

        public const string NameCategory = "Category";
        public const string NameMyModel = "MyModel";

        public const string Success = "Success";
        public const string Error = "Error";

        public const string StatusPending = "Pending"; // в ожидании
        public const string StatusAccepted = "Accepted"; // утвержден
        public const string StatusDenied = "Denied"; // отменен
        public const string StatusInProcess = "In Process"; // в процессе обработки

        public static IEnumerable<string> StatusList =
            new List<string>() { StatusPending, StatusAccepted, StatusInProcess, StatusDenied };
    }
}
