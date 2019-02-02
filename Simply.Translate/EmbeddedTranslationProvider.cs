using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;

namespace Simply.Translate
{
    public class EmbeddedTranslationProvider : BaseTranslationProvider
    {
        public static string FileExtension { get; set; } = ".tr.json";

        private static Dictionary<Assembly, string> Assemblies { get; set; } = new Dictionary<Assembly, string>();
        private static Dictionary<Assembly, List<string>> Files { get; set; } = new Dictionary<Assembly, List<string>>();

        public static void RegisterAssembly(Type type, string prefix = "") => RegisterAssembly(Assembly.GetAssembly(type), prefix);
        public static void RegisterAssembly(Assembly assembly, string prefix = "")
        {
            if(!Assemblies.Keys.Contains(assembly))
            {
                Assemblies.Add(assembly, prefix);
            }
        }

        public override void Initialize()
        {
            Files.Clear();
            foreach(var assembly in Assemblies.Keys)
            {
                var prefix = Assemblies[assembly];
                var compare = !string.IsNullOrEmpty(prefix) ? f => f.StartsWith(prefix) && f.EndsWith(FileExtension) 
                                : (Func<string, bool>)(f => f.EndsWith(FileExtension));
                var files = assembly.GetManifestResourceNames().Where(compare).ToList();

                var available = files.Select(s => s.Replace(FileExtension, string.Empty).Split('.').Where(st => st.Length == 2).LastOrDefault()).Distinct();
                var list = AvailableLanguages.Union(available);
                _languages = list.ToList();
                Files.Add( assembly, files );
            }
        }

        public override void LoadLanguage(string language = null)
        {
            if (language == null) language = TranslationLoader.CurrentLanguage;
            if (AvailableLanguages.Contains(language) && !LoadedLanguages.Contains(language))
            {
                foreach(var assembly in Files.Keys)
                {
                    var files = Files[assembly].Where(f => f.Contains($".{language}.")).ToList();
                    files.ForEach(f => LoadFile(f, assembly, language));
                }
            }
            _loaded.Add(language);
            Translate.AddLoaded(language);
        }

        private static JsonSerializer Serializer = JsonSerializer.CreateDefault();

        /// <summary>
        /// Loads the file from the Assembly
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="fileSource">The file source.</param>
        private void LoadFile(string filename, Assembly fileSource, string language)
        {
            using (var stream = fileSource.GetManifestResourceStream(filename))
            {
                using(var streamReader = new StreamReader(stream))
                {
                    using (var jsonReader = new JsonTextReader(streamReader))
                    {
                        LookupDictionary dictionary = Serializer.Deserialize<LookupDictionary>(jsonReader);
                        Translate.MergeDictionaries(language, dictionary);
                    }
                }
            }
        }

        /// <summary>
        /// Searches for an individual file to load from the initialized sources
        /// </summary>
        /// <param name="filename">The filename.</param>
        public override void LoadFile(string filename, string language = null)
        {
            foreach (var assembly in Files.Keys)
            {
                var actualFile = Files[assembly].FirstOrDefault(f =>f.Contains($".{language ?? Translate.CurrentLanguage}.") &&  f.Contains(filename));
                if (actualFile != null)
                {                    
                    LoadFile(actualFile, assembly, Translate.CurrentLanguage);
                    return;
                }
            }
        }
    }
}
