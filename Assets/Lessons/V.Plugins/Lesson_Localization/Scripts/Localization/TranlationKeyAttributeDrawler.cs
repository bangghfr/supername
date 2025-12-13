// using System;
// using System.Linq;
// using UnityEditor;
// using UnityEngine;
//
// namespace Lessons.Plugins.Lesson_Localization
// {
//     public sealed class TranslationKeyAttributeDrawler : OdinAttributeDrawer<TranslationKeyAttribute, string>
//      {
//          protected override void DrawPropertyLayout(GUIContent label)
//          {
//              var textStorage = LoadTextStorage();
//              if (textStorage == null)
//              {
//                  CallNextDrawer(label);
//                  return;
//              }
//              
//              TextEntity[] entities = textStorage.GetEntities();
//              if (entities == null || entities.Length <= 0)
//              {
//                  CallNextDrawer(label);
//                  return;
//              }
//
//              string[] keys = entities.Select(it => it.key).ToArray();
//              string currentKey = ValueEntry.SmartValue;
//              
//              var index = Array.FindIndex(keys, it => it == currentKey);
//              if (index < 0)
//              {
//                  index = 0;
//              }
//              
//              index = EditorGUILayout.Popup(index, keys);
//              ValueEntry.SmartValue = keys[index];
//          }
//
//          private static TextStorage LoadTextStorage()
//          {
//              var storage = Resources.Load<TextStorage>(nameof(TextStorage));
//              if (storage != null)
//              {
//                  return storage;
//              }
//
//              return Resources.Load<TextStorage>(nameof(TextDictionary));
//          }
//      }
// }
