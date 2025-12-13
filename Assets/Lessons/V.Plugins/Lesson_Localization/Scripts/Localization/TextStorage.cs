using UnityEngine;

namespace Lessons.Plugins.Lesson_Localization
{
    public abstract class TextStorage : ScriptableObject
    {
        public abstract TextEntity[] GetEntities();
    }
}
