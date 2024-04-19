namespace Twitter.Application
{ 

        public static class UserExtension
        {
            public static bool IsPasswordConfirmation(this Microsoft.AspNetCore.Mvc.RazorPages.Models.User user)
            {
                return (user.Password == user.PasswordConfirmation) ? true : false;
            }


        public static string HashPassword(this Models.User user, string password)
        {
            // Реализация хеширования пароля с использованием MD5
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Конвертируем байты обратно в строку
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

    } 
}
