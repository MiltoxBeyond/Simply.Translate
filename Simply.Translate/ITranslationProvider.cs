using System;
using System.Collections.Generic;
using System.Text;

namespace Simply.Translate
{
    public interface ITranslationProvider
    {
        string[] LoadedLanguages { get; }
        string[] AvailableLanguages { get; }

        void Initialize();
        void LoadLanguage(string language = null);
        void LoadFile(string filename, string language = null);
    }
}
