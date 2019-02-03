using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Simply.Translate.WPF.Helpers;

namespace Simply.Translate.WPF
{
    /// <summary>
    /// Translator Markup Extension
    /// Based on https://github.com/codingseb/TranslateMe
    /// </summary>
    /// <seealso cref="System.Windows.Markup.MarkupExtension" />
    [ContentProperty("Key")]
    public class Translator : MarkupExtension
    {
        const string DefaultStringFormat = "{0}";

        private DependencyObject targetObject;
        private DependencyProperty targetProperty;

        public string Key { get; set; }
        public string DefaultValue { get; set; }
        public string Language { get; set; }
        public string StringFormat { get; set; }

        /// <summary>
        /// If set to true, The text will automatically be update when Current Language Change. (use Binding)
        /// If not the property must be updated manually (use single string value).
        /// By default is set to true.
        /// </summary>
        public bool IsDynamic { get; set; } = true;

        /// <summary>
        /// To provide a prefix to add at the begining of the translated text.
        /// </summary>
        public string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// To provide a suffix to add at the end of the translated text.
        /// </summary>
        public string Suffix { get; set; } = string.Empty;

        /// <summary>
        /// Converter to apply on the translated text
        /// </summary>
        public IValueConverter Converter { get; set; } = null;

        public CultureInfo ConverterCulture { get; set; }

        /// <summary>
        /// The parameter to pass to the converter
        /// </summary>
        public object ConverterParameter { get; set; } = null;

        public Translator(string key) : this(key, $"[{key}]", DefaultStringFormat) { }
        public Translator(string key, string defaultValue) : this(key, $"[{key}]", DefaultStringFormat) { }
        public Translator(string key, string defaultValue, string stringFormat)
        {
            Key = key;
            DefaultValue = defaultValue;
            StringFormat = stringFormat;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (service == null)
                return this;

            targetProperty = service.TargetProperty as DependencyProperty;
            targetObject = service.TargetObject as DependencyObject;
            if (targetObject == null || targetProperty == null)
            {
                return this;
            }

            
            try
            {
                if (string.IsNullOrEmpty(Key))
                {
                    if (targetObject != null && targetProperty != null)
                    {

                        string context = targetObject.GetContextByName();
                        string obj = targetObject.FormatForTextId();
                        string property = targetProperty.ToString();

                        Key = $"{context}.{obj}.{property}";
                        
                    }
                    else if (!string.IsNullOrEmpty(DefaultValue))
                    {
                        Key = DefaultValue;
                    }
                }
            }
            catch (InvalidCastException)
            {
                // For Xaml Design Time
                Key = Guid.NewGuid().ToString();
            }
            

            if (IsDynamic)
            {
                Binding binding = new Binding("TranslatedText")
                {
                    Source = new Translation()
                    {
                        Key = Key,
                        DefaultValue = DefaultValue,
                        Language = Language
                    }
                };

                if (Converter != null)
                {
                    binding.Converter = Converter;
                    binding.ConverterParameter = ConverterParameter;
                    //binding.ConverterCulture = ConverterCulture;
                }

                BindingOperations.SetBinding(targetObject, targetProperty, binding);

                return binding.ProvideValue(serviceProvider);
            }
            else
            {
                object result = Prefix + Translate.Get(Language, Key, DefaultValue) + Suffix;

                if (Converter != null)
                {
                    result = Converter.Convert(result, targetProperty.PropertyType, ConverterParameter, ConverterCulture);
                }

                return result;
            }
        }
    }
}
