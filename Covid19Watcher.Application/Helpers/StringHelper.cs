using System;

namespace Covid19Watcher.Application.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// Removes commas
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int SanitizeCommas(this string number) => int.Parse(
            number.Contains(".")
            ?
                String.Join("", number.Split('.'))
            :
                String.Join("", number.Split(','))
        );
        /// <summary>
        /// Ignores string portion after a delimiter
        /// </summary>
        /// <param name="text"></param>
        /// <param name="afterReference"></param>
        /// <returns></returns>
        public static string IgnoreAfter(this string text, string afterReference) => text.Substring(0, text.IndexOf(afterReference)).Trim();
    }
}