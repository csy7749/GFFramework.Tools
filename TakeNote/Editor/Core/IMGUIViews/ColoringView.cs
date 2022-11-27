
using UnityEditor;
using UnityEngine;

namespace FuguFirecracker.TakeNote
{
    internal class ColoringView
    {
        private const float LABEL_WIDTH = 74;
        internal Color BackgroundColor;
        internal Color ContentColor;

        public ColoringView(Color backgroundColor, Color contentColor)
        {
            BackgroundColor = backgroundColor;
            ContentColor = contentColor;
     }
        internal void Draw()
        {
            GUILayout.Space(6);
            EditorGUIUtility.labelWidth = LABEL_WIDTH;
            BackgroundColor = EditorGUILayout.ColorField("Background",BackgroundColor);
            ContentColor = EditorGUILayout.ColorField("Text", ContentColor);
            GUILayout.Space(4);

            var bgColorCache = GUI.backgroundColor;
            var contentColorCache = GUI.contentColor;

            GUI.backgroundColor = BackgroundColor;
            GUI.contentColor = ContentColor;
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(LABEL_WIDTH);
            EditorGUILayout.LabelField("Hey, check it out !", Style.ExampleStyle, GUILayout.Height(24));
            GUILayout.Space(24);
            EditorGUILayout.EndHorizontal();

            GUI.backgroundColor = bgColorCache;
            GUI.contentColor = contentColorCache;
        }
    }
}