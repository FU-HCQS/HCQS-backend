using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Util
{
    public class CaseConverter
    {
        //kabab-case to PascalCase
        public static string ConvertKebabToPascal(string str)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string[] words = str.Split('-');
            for (int i = 1; i < words.Length; i++)
            {
                words[i] = textInfo.ToTitleCase(words[i]);
            }

            return string.Join("", words);
        }

        //PascalCase to kabab-case
        public static string ConvertPascalToKebab(string str)
        {
            string[] words = Regex.Split(str, @"(?<!^)(?=[A-Z])");

            // Join the words with hyphens and convert to lowercase
            string kebabCase = string.Join("-", words).ToLower();

            return kebabCase;
        }

        public static string ConvertCamelToKebab(string camelCase)
        {
            // Insert hyphens before uppercase letters and convert to lowercase
            string kebabCase = Regex.Replace(camelCase, @"(?<!^)(?=[A-Z])", "-").ToLower();
            return kebabCase;
        }

        public static string ConvertKebabToCamel(string kebabCase)
        {
            // Remove hyphens and capitalize the first letter of each word
            string[] words = kebabCase.Split('-');
            for (int i = 1; i < words.Length; i++)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
            }
            string camelCase = string.Join("", words);
            return camelCase;
        }
    }
}
