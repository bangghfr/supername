using System;
using TMPro;
using UnityEngine;

namespace Lessons.Plugins.Lesson_Localization
{
    [Serializable]
    public sealed class TextLanguageListener : ILanguageListener
    {
        [SerializeField]
        private TextMeshProUGUI text;

        [TranslationKey]
        [SerializeField]
        private string key;

        // void ILanguageListener.OnLanguageChanged(SystemLanguage language)
        // {
        //     text.text = LocalizationManager.GetText(key, language);
        // }
        
        public void OnLanguageChanged(SystemLanguage language)
        {
            text.text = LocalizationManager.GetText(key, language);
        }
    }
}