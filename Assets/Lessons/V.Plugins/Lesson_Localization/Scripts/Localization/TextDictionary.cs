using System;
using UnityEngine;

namespace Lessons.Plugins.Lesson_Localization
{
    [CreateAssetMenu(
        fileName = "Text Dictionary",
        menuName = "Lessons/New Text Dictionary"
    )]
    public sealed class TextDictionary : TextStorage
    {
        [TextArea]
        [SerializeField]
        public string uri;
    
        [SerializeField]
        public TextEntity[] entities;

        public override TextEntity[] GetEntities()
        {
            return entities ?? Array.Empty<TextEntity>();
        }
    }
}
