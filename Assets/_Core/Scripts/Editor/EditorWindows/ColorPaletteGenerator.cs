using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace UrbanFox.Editor
{
    public class ColorPaletteGenerator : EditorWindow
    {
        private const int k_colorsPerRow = 4;
        private const int k_maxNumberOfColorsPerPalette = k_colorsPerRow * k_colorsPerRow;
        private const int k_imageSize = 1024;
        private readonly Color[] m_cachedColors = new Color[k_maxNumberOfColorsPerPalette + 1];

        public struct EditorData
        {
            public List<Color> Colors;
            public Color BackgroundColor;
            public ImageFormat ImageFormat;
            public Texture2D ReferenceTexture;
        }

        [Serializable]
        public enum ImageFormat
        {
            [InspectorName("JPG (Transparency Not Supported)")]
            JPG,
            [InspectorName("PNG (Supports Transparency)")]
            PNG
        }

        [SerializeField]
        private List<Color> m_colors = new List<Color>();

        [SerializeField]
        private Color m_backgroundColor = Color.black;

        [SerializeField]
        private ImageFormat m_imageFormat;

        [SerializeField]
        private Texture2D m_referenceTexture;

        private Texture2D m_texturePreview;
        private bool m_isPreviewDirty;

        private Vector2 m_scroll;
        private SerializedObject m_target;
        private SerializedProperty m_colorsProperty;
        private EditorData m_editorData;

        private static string EditorPrefsKey => $"{Application.companyName}/{Application.productName}/{typeof(ColorPaletteGenerator)}";

        [MenuItem("OwO/Window/Color Palette Generator...")]
        private static void OpenWindow()
        {
            var window = GetWindow<ColorPaletteGenerator>();
            window.titleContent = new GUIContent("Color Palette Generator");
            window.Show();
        }

        private void OnEnable()
        {
            m_target = new SerializedObject(this);
            m_colorsProperty = m_target.FindProperty(nameof(m_colors));
            m_texturePreview = new Texture2D(k_imageSize, k_imageSize);
            m_isPreviewDirty = true;
            if (EditorPrefs.HasKey(EditorPrefsKey))
            {
                m_editorData = JsonUtility.FromJson<EditorData>(EditorPrefs.GetString(EditorPrefsKey));
            }
            else
            {
                m_editorData = new EditorData();
                m_editorData.BackgroundColor = Color.black;
            }
            m_editorData = EditorPrefs.HasKey(EditorPrefsKey) ? JsonUtility.FromJson<EditorData>(EditorPrefs.GetString(EditorPrefsKey)) : new EditorData();
            m_colors = m_editorData.Colors;
            m_backgroundColor = m_editorData.BackgroundColor;
            m_imageFormat = m_editorData.ImageFormat;
            m_referenceTexture = m_editorData.ReferenceTexture;
        }

        private void OnDisable()
        {
            EditorPrefs.SetString(EditorPrefsKey, JsonUtility.ToJson(m_editorData));
        }

        private void OnGUI()
        {
            m_scroll = GUILayout.BeginScrollView(m_scroll);
            m_backgroundColor = EditorGUILayout.ColorField("Background Color", m_backgroundColor);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_colorsProperty);
            GUILayoutExtensions.HorizontalLine();

            if (m_colorsProperty.arraySize > k_maxNumberOfColorsPerPalette)
            {
                m_colorsProperty.arraySize = k_maxNumberOfColorsPerPalette;
                FoxyLogger.LogWarning($"Only up to {k_maxNumberOfColorsPerPalette} colors can be added to a color palette.");
            }

            for (int i = 0; i < m_colors.Count; i++)
            {
                var colorToCompare = m_colors[i];
                if (!m_cachedColors[i].IsApproximately(colorToCompare))
                {
                    m_cachedColors[i] = colorToCompare;
                    m_isPreviewDirty = true;
                }
            }
            if (!m_cachedColors[m_colors.Count].IsApproximately(m_backgroundColor))
            {
                m_cachedColors[m_colors.Count] = m_backgroundColor;
                m_isPreviewDirty = true;
            }

            GUILayout.Label("Color Palette Preview", EditorStyles.boldLabel);
            if (m_isPreviewDirty)
            {
                RedrawColorPaletteTexture();
                m_isPreviewDirty = false;
            }
            var rect = GUILayoutUtility.GetLastRect();
            rect.y += EditorGUIUtility.singleLineHeight;
            rect.width = 100;
            rect.height = 100;
            GUI.DrawTexture(rect, m_texturePreview);
            GUI.DrawTexture(rect, m_texturePreview);
            EditorGUILayout.Space(rect.height + EditorGUIUtility.singleLineHeight);
            GUILayoutExtensions.HorizontalLine();

            GUILayout.Label("Save Options", EditorStyles.boldLabel);
            m_imageFormat = (ImageFormat)EditorGUILayout.EnumPopup("Image Format", m_imageFormat);
            if (GUILayout.Button("Export to Image File..."))
            {
                var extension = m_imageFormat == ImageFormat.JPG ? "jpg" :
                    m_imageFormat == ImageFormat.PNG ? "png" :
                    "jpg";
                var desiredPath = AssetDatabase.GenerateUniqueAssetPath($"Assets/ColorPalette.{extension}");
                var exportedFilePath = EditorUtility.SaveFilePanel("Save Palette As", "Assets/", Path.GetFileNameWithoutExtension(desiredPath), extension);
                if (!exportedFilePath.IsNullOrEmpty())
                {
                    var bytes = m_imageFormat == ImageFormat.JPG ? m_texturePreview.EncodeToJPG() :
                        m_imageFormat == ImageFormat.PNG ? m_texturePreview.EncodeToPNG() :
                        m_texturePreview.EncodeToJPG();
                    File.WriteAllBytes(exportedFilePath, bytes);
                    AssetDatabase.Refresh();
                    if (exportedFilePath.StartsWith(Application.dataPath))
                    {
                        exportedFilePath = $"Assets{exportedFilePath.Substring(Application.dataPath.Length)}";
                    }
                    m_referenceTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(exportedFilePath);
                }
            }
            EditorGUILayout.Space();
            GUILayoutExtensions.HorizontalLine();

            GUILayout.Label("Reference Image File", EditorStyles.boldLabel);
            m_referenceTexture = (Texture2D)EditorGUILayout.ObjectField("Reference Texture", m_referenceTexture, typeof(Texture2D), allowSceneObjects: false);
            GUI.enabled = m_referenceTexture != null;
            if (GUILayout.Button("Generate Color Palette from Reference Image"))
            {
                ReadColorPaletteFromReferenceTexture();
            }
            GUI.enabled = true;

            m_editorData.Colors = m_colors;
            m_editorData.BackgroundColor = m_backgroundColor;
            m_editorData.ImageFormat = m_imageFormat;
            m_editorData.ReferenceTexture = m_referenceTexture;

            GUILayout.EndScrollView();
            m_target.ApplyModifiedProperties();
        }

        private void RedrawColorPaletteTexture()
        {
            for (int i = 0; i < m_texturePreview.width; i++)
            {
                for (int j = 0; j < m_texturePreview.height; j++)
                {
                    m_texturePreview.SetPixel(i, j, m_backgroundColor);
                }
            }
            if (!m_colors.IsNullOrEmpty())
            {
                int xOffset = 0;
                int yOffset = 0;
                int numberOfColorsOnX = 0;
                int colorGridSize = k_imageSize / k_colorsPerRow;
                foreach (var color in m_colors)
                {
                    for (int i = 0; i < colorGridSize; i++)
                    {
                        for (int j = 0; j < colorGridSize; j++)
                        {
                            m_texturePreview.SetPixel(xOffset + i, m_texturePreview.height - 1 - (yOffset + j), color);
                        }
                    }
                    numberOfColorsOnX++;
                    if (numberOfColorsOnX >= k_colorsPerRow)
                    {
                        numberOfColorsOnX = 0;
                        yOffset += colorGridSize;
                    }
                    xOffset = numberOfColorsOnX * colorGridSize;
                }
            }
            m_texturePreview.Apply();
        }

        private void ReadColorPaletteFromReferenceTexture()
        {
            int gridWidth = m_referenceTexture.width / k_colorsPerRow;
            int gridHeight = m_referenceTexture.height / k_colorsPerRow;
            int xOffset = 0;
            int yOffset = gridHeight;
            int numberOfColorsOnX = 0;
            Texture2D tempReadableTexturePreview = CreateReadabeTexture2D(m_referenceTexture);
            while (m_colorsProperty.arraySize < k_maxNumberOfColorsPerPalette)
            {
                m_colorsProperty.arraySize++;
                m_colorsProperty.GetArrayElementAtIndex(m_colorsProperty.arraySize - 1).colorValue = m_backgroundColor;
            }
            for (int i = 0; i < m_colorsProperty.arraySize; i++)
            {
                var pixels = tempReadableTexturePreview.GetPixels(xOffset, tempReadableTexturePreview.height - yOffset, gridWidth, gridHeight);
                m_colorsProperty.GetArrayElementAtIndex(i).colorValue = pixels.GetAverageColor();
                numberOfColorsOnX++;
                if (numberOfColorsOnX >= k_colorsPerRow)
                {
                    numberOfColorsOnX = 0;
                    yOffset += gridHeight;
                }
                xOffset = numberOfColorsOnX * gridWidth;
            }

            // Remove redundant background colors
            while (m_colorsProperty.arraySize > 1 && m_colorsProperty.GetArrayElementAtIndex(m_colorsProperty.arraySize - 1).colorValue.IsApproximately(m_backgroundColor))
            {
                m_colorsProperty.arraySize--;
            }
            m_isPreviewDirty = true;
        }

        // https://qiita.com/Katumadeyaruhiko/items/c2b9b4ccdfe51df4ad4a
        private Texture2D CreateReadabeTexture2D(Texture2D texture2d)
        {
            RenderTexture renderTexture = RenderTexture.GetTemporary(
                        texture2d.width,
                        texture2d.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(texture2d, renderTexture);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTexture;
            Texture2D readableTextur2D = new Texture2D(texture2d.width, texture2d.height);
            readableTextur2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            readableTextur2D.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTexture);
            return readableTextur2D;
        }
    }
}
