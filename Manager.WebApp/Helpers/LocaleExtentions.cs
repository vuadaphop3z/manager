using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;

namespace Manager.WebApp.Helpers
{
    public enum EnumLanguage
    {
        [Display(Name = "Vietnamese")]
        vi,
        [Display(Name = "English")]
        en
    }

    public static class Extensions
    {
        /// <summary>
        ///     A generic extension method that aids in reflecting 
        ///     and retrieving any attribute that is applied to an `Enum`.
        /// </summary>
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
                where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString()).First()
                            .GetCustomAttribute<TAttribute>();
        }
    }
}