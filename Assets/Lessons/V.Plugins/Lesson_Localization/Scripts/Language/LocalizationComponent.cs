using UnityEngine;

namespace Lessons.Plugins.Lesson_Localization
{
    public sealed class LocalizationComponent : MonoBehaviour
    {
        // [SerializeReference]
        // private ILanguageListener[] _listeners;
        
        [SerializeField]
        private TextLanguageListener _listener;
    
        private void OnEnable()
        {
            UpdateLanguage(LanguageManager.Language);
            LanguageManager.OnLanguageChanged += UpdateLanguage;
        }

        private void OnDisable()
        {
            LanguageManager.OnLanguageChanged -= UpdateLanguage;            
        }

        private void UpdateLanguage(SystemLanguage language)
        {
           _listener.OnLanguageChanged(language);
        }
    }
}