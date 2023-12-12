using UnityEngine;

namespace UrbanFox
{
    public static class GUILayoutExtensions
    {
        private const int _halfSpace = 3;
        private static GUIStyle _divider = null;

        private static GUIStyle Divider
        {
            get
            {
                if (_divider == null)
                {
                    var whiteTexture = new Texture2D(1, 1);
                    whiteTexture.SetPixel(0, 0, Color.white);
                    whiteTexture.Apply();
                    _divider = new GUIStyle();
                    _divider.normal.background = whiteTexture;
                    _divider.margin = new RectOffset(2, 2, 2, 2);
                }
                return _divider;
            }
        }

        public static bool ColoredButton(GUIContent content, Color color, GUIStyle style, params GUILayoutOption[] options)
        {
            var cachedBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            if (GUILayout.Button(content, style, options))
            { 
                GUI.backgroundColor = cachedBackgroundColor;
                return true;
            }
            GUI.backgroundColor = cachedBackgroundColor;
            return false;
        }

        public static bool ColoredButton(string label, Color color, GUIStyle style, params GUILayoutOption[] options)
        {
            return ColoredButton(new GUIContent(label), color, style, options);
        }

        public static bool ColoredButton(string label, Color color, params GUILayoutOption[] options)
        {
            return ColoredButton(label, color, GUI.skin.button, options);
        }

        public static void HorizontalLine(int height, Color color, float margin)
        {
            Divider.fixedHeight = height;
            var cachedGUIColor = GUI.color;
            GUI.color = color;
            GUILayout.Space(margin);
            GUILayout.Box(GUIContent.none, Divider);
            GUILayout.Space(margin);
            GUI.color = cachedGUIColor;
        }

        public static void HorizontalLine(int height, Color color)
        {
            HorizontalLine(height, color, _halfSpace);
        }

        public static void HorizontalLine(int height = 1)
        {
            HorizontalLine(height, Color.gray);
        }

        public static void HorizontalLine(Color color)
        {
            HorizontalLine(1, color);
        }
    }
}
