using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Services.Services
{
    public class CsvHelper
    {
        public static string GetCsv<T>(List<T> list, List<string> headers = null)
        {

            var rows = new StringBuilder();

            if (headers != null)
            {
                rows.Append(string.Join(",", headers));
                rows.Append(Environment.NewLine);
            }
            var properties = typeof(T).GetProperties();

            foreach (var item in list)
            {
                for (var i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];
                    var propertyValue = property.GetValue(item, null)?.ToString();
                    rows.Append(i == properties.Length - 1 ? $"{propertyValue}" : $"{propertyValue},");
                }

                rows.Append(Environment.NewLine);
            }

            return rows.ToString();
        }

        public static byte[] GetCsvBytes<T>(List<T> list, List<string> headers = null)
        {
            var csv = GetCsv(list, headers);
            return Encoding.UTF8.GetBytes(csv);
        }
    }
}
