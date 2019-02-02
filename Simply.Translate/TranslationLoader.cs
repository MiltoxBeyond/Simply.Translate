using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simply.Translate
{
    public static class TranslationLoader
    {
        public static event EventHandler<string> OnTranslationChanged;

        public static List<ITranslationProvider> TranslationProviders { get; private set; } = new List<ITranslationProvider>();
        public static List<string> AvailableLanguages { get => Translate.Instance.Available.ToList(); } 
        public static List<string> LoadedLanguages { get => Translate.Instance.Loaded.ToList(); }

        public static string CurrentLanguage { get; private set; }        

        public static void Initialize(params ITranslationProvider[] providers)
        {
            TranslationProviders.Clear();
            TranslationProviders.AddRange(providers);
            TranslationProviders.ForEach(provider => provider.Initialize());
        }

        public static void LoadLanguage(string language = null)
        {
            if(CurrentLanguage != language)
            {
                TranslationProviders.ForEach(p => { if(p.LoadedLanguages.Contains(language)) p.LoadLanguage(language); });                    
            }
        }

        public static void LoadFile(string filename, string language = null)
        {
            TranslationProviders.ForEach(p => p.LoadFile(filename, language));
        }
    }
}
