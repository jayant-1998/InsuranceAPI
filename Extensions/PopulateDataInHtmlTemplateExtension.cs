using System.Reflection;

namespace InsuranceAPI.Extensions
{
    public static class PopulateDataInHtmlTemplateExtension
    {
        public static string PopulateDataInHtmlTemplate<T2>(this string htmlTemplate, T2 user)
        {
            PropertyInfo[] properties = user.GetType().GetProperties();
            foreach ( var property in properties )
            {
                string placeholder = $"{{{{{property.Name}}}}}";
                object value = property.GetValue(user);
                string valueString = value != null ? value.ToString() : string.Empty;
                htmlTemplate = htmlTemplate.Replace(placeholder, valueString);
            }
            return htmlTemplate;
        }
    }
}
