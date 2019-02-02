using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Simply.Translate
{
    public class Translate : INotifyPropertyChanged
    {
        #region //Statics
        private static Translate _instance;
        public static Translate Instance { get => _instance ?? (_instance = new Translate()); }

        public static string CurrentLanguage { get => TranslationLoader.CurrentLanguage; }
        #endregion


        public ObservableCollection<string> Available { get; private set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Loaded { get; private set; } = new ObservableCollection<string>();
        private TranslationDictionary Dictionary { get; set; } = new TranslationDictionary();

        internal static void AddAvailable(string available)
        {
            if (!Instance.Available.Contains(available))
                Instance.Available.Add(available);
        }
        
        internal static void AddLoaded(string loaded)
        {
            if (!Instance.Loaded.Contains(loaded))
                Instance.Loaded.Add(loaded);
        }

        /// <summary>
        /// Merges the loaded dictionary.  If a key for a language exists it will 
        /// replace the value with whatever is loaded, therefore, order of load matters.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="lookup">New Dictionary to merge</param>
        internal static void MergeDictionaries(string language, LookupDictionary lookup)
        {
            if (Instance.Dictionary.ContainsKey(language))
            {
                foreach (var key in lookup.Keys)
                {
                    
                    if (Instance[language][key] == null) Instance[language].Add(language, lookup[key]);
                    else Instance[language][key] = lookup[key];//TODO: Add logging of duplicate keys
                }
            }
            else
            {
                Instance.Dictionary.Add(language, lookup);
            }            
        }

        public LookupDictionary this[string language] => Dictionary.ContainsKey(language) ? Dictionary[language] : null;

        public string this[string language,string key]
            => this[language]?.ContainsKey(key) == true ? this[language][key] : null;

        public static string Get(string language, string key, string defaultValue = "")
            => Instance[language ?? CurrentLanguage, key] ?? defaultValue;

        public static string Get(string key, string defaultValue = "") 
            => Instance[CurrentLanguage, key] ?? defaultValue;

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
