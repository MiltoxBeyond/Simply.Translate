using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Simply.Translate
{
    public class Translation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        public Translation()
        {
            TranslationLoader.OnTranslationChanged += TranslationLanguageChanged;
        }

        private void TranslationLanguageChanged(object sender, string e)
        {
            if (string.IsNullOrEmpty(Language))
                NotifyPropertyChanged(nameof(TranslatedText));
        }

        ~Translation()
        {
            TranslationLoader.OnTranslationChanged -= TranslationLanguageChanged;
        }

        public string Key { get; set; }
        public string Language { get; set; }
        public string DefaultValue { get; set; }

        public string TranslatedText { get => Translate.Get(Language, Key, DefaultValue ?? $"[{Key}"); }

        public void NotifyPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
