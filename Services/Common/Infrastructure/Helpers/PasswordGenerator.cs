using System;
using System.Linq;
using System.Text;

namespace Common.Infrastructure.Helpers
{
    public class PasswordGenerator
    {
        static public string GeneratePassword(UserLoginPolicy userLoginPolicy)
        {
            //var options = new { RequiredLength = 8, RequireNonAlphanumeric = true, RequireDigit = true, RequireLowercase = true, RequireUppercase = true };
            int length = userLoginPolicy.RequiredLength;
            bool nonAlphanumeric = userLoginPolicy.RequireNonAlphanumeric;
            bool digit = userLoginPolicy.RequireDigit;
            bool lowercase = userLoginPolicy.RequireLowercase;
            bool uppercase = userLoginPolicy.RequireUppercase;

            StringBuilder password = new StringBuilder();
            Random random = new Random();

            while (password.Length < length)
            {
                char c = (char)random.Next(33, 126);
                password.Append(c);
                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }
            if (nonAlphanumeric)
                password.Append((char)random.Next(33, 48));
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));

            return password.ToString();
        }
        static public bool ValidPasssword(UserLoginPolicy userLoginPolicy, string password)
        {
            if (password.Length < userLoginPolicy.RequiredLength)
                return false;
            if (userLoginPolicy.RequireLowercase && password.Any(char.IsLower) == false)
                return false;
            if (userLoginPolicy.RequireUppercase && password.Any(char.IsUpper) == false)
                return false;
            if (userLoginPolicy.RequireDigit && password.Any(char.IsDigit) == false)
                return false;
            if (userLoginPolicy.RequireNonAlphanumeric && password.All(char.IsLetterOrDigit) == true)
                return false;

            return true;
        }
    }
}
