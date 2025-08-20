using System;
using System.Text;

namespace Common.Infrastructure.Helpers
{
    public class Base36String
    {
        static public string ToString(ulong value)
        {
            const string base36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var sb = new StringBuilder(13);
            do
            {
                sb.Insert(0, base36[(byte)(value % 36)]);
                value /= 36;
            } while (value != 0);
            return sb.ToString();
        }

        static public string ToString(DateTime dt)
        {
            const string base36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            ulong value = ulong.Parse(dt.ToString("MMddhhmmssff"));
            var sb = new StringBuilder(13);
            do
            {
                sb.Insert(0, base36[(byte)(value % 36)]);
                value /= 36;
            } while (value != 0);
            return sb.ToString();
        }

        static public long ToLong(string input)
        {
            const string base36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
            var reversed = new string(charArray).ToUpper();
            long result = 0;
            int pos = 0;
            foreach (char c in reversed)
            {
                result += base36.IndexOf(c) * (long)Math.Pow(36, pos);
                pos++;
            }
            return result;
        }
    }
}
