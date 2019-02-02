using System;
using System.Collections.Generic;
using System.Text;

namespace Simply.Translate
{
    /// <summary>
    /// Extra methods to simplify access to Child Translations
    /// </summary>
    /// <seealso cref="Simply.Translate.ITranslationProvider" />
    public class BaseTranslationProvider : ITranslationProvider
    {
        public string[] LoadedLanguages { get => _loaded.ToArray(); }
        protected List<string> _loaded = new List<string>();

        public string[] AvailableLanguages { get => _languages.ToArray(); }
        protected List<string> _languages = new List<string>();

        public virtual void Initialize() { }
        public virtual void LoadLanguage(string language = null){}
        public virtual void LoadFile(string filename, string language = null) { }
    }
}
