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
            String.Join("", number.Split(','))
        );
    }
}