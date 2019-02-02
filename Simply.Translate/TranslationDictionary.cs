using System;
using System.Collections.Generic;
using System.Text;

namespace Simply.Translate
{
    public class LookupDictionary : ObservableDictionary<string, string> { }
    public class TranslationDictionary : ObservableDictionary<string, LookupDictionary> {
        public override void Add(string key, LookupDictionary value)
        {
            base.Add(key, value);
            value.CollectionChanged += (o,e) => NotifyObserversOfChange();
        }

        protected override void UpdateWithNotification(string key, LookupDictionary value)
        {
            base.UpdateWithNotification(key, value);
            value.CollectionChanged += (o, e) => NotifyObserversOfChange();
        }
    }    
}
