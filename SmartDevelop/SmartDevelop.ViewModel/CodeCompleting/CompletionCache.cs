using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.CodeLanguages;
using Archimedes.Patterns;

namespace SmartDevelop.ViewModel.CodeCompleting
{
    public class CompletionCache : Singleton<CompletionCache>
    {
        Dictionary<CodeLanguage, LanguageCompletionCache> _cachemap = new Dictionary<CodeLanguage, LanguageCompletionCache>();


        public LanguageCompletionCache this[CodeLanguage languge] {
            
            get {
                if(!_cachemap.ContainsKey(languge)) {
                    return null;
                }
                return _cachemap[languge];
            }

            set {
                if(!_cachemap.ContainsKey(languge)) {
                    _cachemap.Add(languge, value);
                }else
                    _cachemap[languge] = value;
            }
        }

        public class LanguageCompletionCache
        {
            List<CompletionItem> _staticCompletionItems = new List<CompletionItem>();

            public void AddStatic(CompletionItem item) {
                _staticCompletionItems.Add(item);
            }
            public void AddStatic(IEnumerable<CompletionItem> items) {
                _staticCompletionItems.AddRange(items);
            }

            public IEnumerable<CompletionItem> GetAllStaticCompletionItems() {
                return new List<CompletionItem>(_staticCompletionItems);
            }
        }
    }
}
