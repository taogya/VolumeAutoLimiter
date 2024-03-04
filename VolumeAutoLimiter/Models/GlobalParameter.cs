using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumeAutoLimiter.Models
{
    public static class GlobalParameter
    {
        public static string DefaultCultureCode { get; } = "en";

        public static string GetLanguagePath() => GetLanguagePath(DefaultCultureCode);
        public static string GetLanguagePath(string cultureCode) => @$"Resources/language/Dictionary.{cultureCode}.xaml";
    }
}
