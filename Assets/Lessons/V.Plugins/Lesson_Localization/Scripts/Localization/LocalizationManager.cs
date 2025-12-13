using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lessons.Plugins.Lesson_Localization
{
    public sealed class LocalizationManager : MonoBehaviour
    {
        private static LocalizationManager instance;

        [FormerlySerializedAs("textConfig")]
        [SerializeField]
        private TextStorage textStorage;

        private ITranslator<string> textTranslator;

        private void Awake()
        {
            if (instance != null)
            {
                throw new Exception("Localization Manager is already exists!");
            }
            
            instance = this;
            if (textStorage == null)
            {
                Debug.LogWarning("LocalizationManager: text storage is not assigned.");
            }

            var entities = textStorage != null
                ? textStorage.GetEntities()
                : Array.Empty<TextEntity>();
            
            textTranslator = new TextTranslator(entities);
        }

        private void OnDestroy()
        {
            instance = null;
        }
        
        public static string GetText(string key, SystemLanguage language)
        {
            if (instance != null)
            {
                return instance.textTranslator.GetTranlation(key, language);                
            }

            return key;
        }
    }
}
