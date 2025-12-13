using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lessons.Plugins.Lesson_Localization
{
    [CreateAssetMenu(
        fileName = "Json Text Storage",
        menuName = "Lessons/New Json Text Storage"
    )]
    public sealed class JsonTextStorage : TextStorage
    {
        [SerializeField]
        private TextAsset jsonFile;

        private TextEntity[] cache;

        public override TextEntity[] GetEntities()
        {
            if (cache == null)
            {
                cache = ReadEntities();
            }

            return cache;
        }

        private TextEntity[] ReadEntities()
        {
            if (jsonFile == null)
            {
                Debug.LogWarning("JsonTextStorage: json file is not assigned.");
                return Array.Empty<TextEntity>();
            }

            var json = jsonFile.text;
            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogWarning($"JsonTextStorage: json file '{jsonFile.name}' is empty.");
                return Array.Empty<TextEntity>();
            }

            try
            {
                var normalizedJson = Normalize(json);
                var wrapper = JsonUtility.FromJson<JsonTextEntityCollection>(normalizedJson);
                if (wrapper == null || wrapper.entities == null || wrapper.entities.Length == 0)
                {
                    return Array.Empty<TextEntity>();
                }

                return Convert(wrapper.entities);
            }
            catch (Exception exception)
            {
                Debug.LogError($"JsonTextStorage: failed to parse '{jsonFile.name}'. {exception.Message}");
                return Array.Empty<TextEntity>();
            }
        }

        private static string Normalize(string json)
        {
            var trimmed = json.Trim();
            if (trimmed.StartsWith("[", StringComparison.Ordinal))
            {
                return $"{{\"entities\":{trimmed}}}";
            }

            return trimmed;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            cache = null;
        }
#endif

        private static TextEntity[] Convert(JsonTextEntity[] source)
        {
            var count = source.Length;
            var result = new TextEntity[count];
            for (var i = 0; i < count; i++)
            {
                var item = source[i];
                result[i] = new TextEntity
                {
                    key = item.key ?? string.Empty,
                    translations = Convert(item.translations)
                };
            }

            return result;
        }

        private static LocalizedString[] Convert(JsonLocalizedString[] source)
        {
            if (source == null || source.Length == 0)
            {
                return Array.Empty<LocalizedString>();
            }

            var result = new List<LocalizedString>(source.Length);
            foreach (var entry in source)
            {
                if (!TryParseLanguage(entry.language, out var language))
                {
                    Debug.LogWarning($"JsonTextStorage: unknown language '{entry.language}'.");
                    continue;
                }

                var value = entry.value ?? string.Empty;
                result.Add(new LocalizedString
                {
                    language = language,
                    value = value
                });
            }

            return result.ToArray();
        }

        private static bool TryParseLanguage(string text, out SystemLanguage language)
        {
            language = default;
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            if (Enum.TryParse(text, true, out language))
            {
                return true;
            }

            if (int.TryParse(text, out var raw) && Enum.IsDefined(typeof(SystemLanguage), raw))
            {
                language = (SystemLanguage)raw;
                return true;
            }

            return false;
        }

        [Serializable]
        private sealed class JsonLocalizedString
        {
            public string language;
            public string value;
        }

        [Serializable]
        private sealed class JsonTextEntity
        {
            public string key;
            public JsonLocalizedString[] translations;
        }

        [Serializable]
        private sealed class JsonTextEntityCollection
        {
            public JsonTextEntity[] entities;
        }
    }
}
