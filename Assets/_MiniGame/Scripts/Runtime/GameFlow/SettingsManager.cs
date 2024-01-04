using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace UrbanFox.MiniGame
{
    public class SettingsManager : RuntimeManager<SettingsManager>
    {
        [Serializable]
        public struct SettingsData
        {
            public int LanguageIndex;
            public int ScreenWidth;
            public int ScreenHeight;
            public bool IsFullscreen;
            public float Brightness;
            public float Volume;
            public bool EnableControllerRumble;
            public string InputBindingsOverride;
        }

        [SerializeField]
        private string m_settingsFileName;

        [SerializeField]
        private bool m_verboseLogging;

        [SerializeField]
        private bool m_writeFileWithPrettyFormat = true;

        [SerializeField, Required]
        private Toggle m_fullscreenToggle;

        [SerializeField, Required]
        private Slider m_gammaSlider;

        [SerializeField, Required]
        private Toggle m_enableControllerRumble;

        [SerializeField]
        private AnimationCurve m_sliderToActualGammaCurve = AnimationCurve.Linear(-1, -0.1f, 1, 0.1f);

        [SerializeField]
        private SystemLanguage[] m_defaultLanguageIndexList;

        [SerializeField, NonEditable]
        private SettingsData m_currentSettings;

        private bool m_isSettingsDirty = false;

        public string SettingsFilePath => Path.Combine(Application.persistentDataPath, m_settingsFileName);
        public AnimationCurve SliderToActualGammaCurve => m_sliderToActualGammaCurve;

        public int LanguageIndex
        {
            get => m_currentSettings.LanguageIndex;
            set
            {
                m_currentSettings.LanguageIndex = value;
                m_isSettingsDirty = true;
            }
        }

        public int ScreenWidth
        {
            get => m_currentSettings.ScreenWidth;
            set
            {
                m_currentSettings.ScreenWidth = value;
                m_isSettingsDirty = true;
            }
        }

        public int ScreenHeight
        {
            get => m_currentSettings.ScreenHeight;
            set
            {
                m_currentSettings.ScreenHeight = value;
                m_isSettingsDirty = true;
            }
        }

        public bool IsFullscreen
        {
            get => m_currentSettings.IsFullscreen;
            set
            {
                m_currentSettings.IsFullscreen = value;
                m_isSettingsDirty = true;
            }
        }

        public float Brightness
        {
            get => m_currentSettings.Brightness;
            set
            {
                m_currentSettings.Brightness = value;
                m_isSettingsDirty = true;
            }
        }

        public float Volume
        {
            get => m_currentSettings.Volume;
            set
            {
                m_currentSettings.Volume = value;
                m_isSettingsDirty = true;
            }
        }

        public bool EnableControllerRumble
        {
            get => m_currentSettings.EnableControllerRumble;
            set
            {
                m_currentSettings.EnableControllerRumble = value;
                m_isSettingsDirty = true;
            }
        }

        public string InputBindingsOverride
        {
            get => m_currentSettings.InputBindingsOverride;
            set
            {
                m_currentSettings.InputBindingsOverride = value;
                m_isSettingsDirty = true;
            }
        }

        public void ResetLanguageSettings()
        {
            if (m_defaultLanguageIndexList.IsNullOrEmpty())
            {
                LanguageIndex = 0;
            }
            else
            {
                for (int i = 0; i < m_defaultLanguageIndexList.Length; i++)
                {
                    if (m_defaultLanguageIndexList[i] == Application.systemLanguage)
                    {
                        LanguageIndex = i;
                        return;
                    }
                }
            }
        }

        public void ResetGraphicSettings()
        {
            if (!Screen.resolutions.IsNullOrEmpty())
            {
                ScreenWidth = Screen.resolutions[Screen.resolutions.Length - 1].width;
                ScreenHeight = Screen.resolutions[Screen.resolutions.Length - 1].height;
            }
            IsFullscreen = true;
            Brightness = 0;
            m_fullscreenToggle.isOn = IsFullscreen;
            m_gammaSlider.value = Brightness;
            Screen.SetResolution(ScreenWidth, ScreenHeight, IsFullscreen);
        }

        public void ResetAudioSettings()
        {
            Volume = 1;
        }

        public void ResetControlSettings()
        {
            EnableControllerRumble = true;
            m_enableControllerRumble.isOn = EnableControllerRumble;
            InputBindingsOverride = string.Empty;
        }

        public void ResetAllSettings()
        {
            ResetLanguageSettings();
            ResetGraphicSettings();
            ResetAudioSettings();
            ResetControlSettings();
        }

        public override void Awake()
        {
            base.Awake();
            if (File.Exists(SettingsFilePath))
            {
                try
                {
                    m_currentSettings = JsonUtility.FromJson<SettingsData>(File.ReadAllText(SettingsFilePath));
                    if (m_verboseLogging)
                    {
                        FoxyLogger.Log($"File loaded from {SettingsFilePath}.");
                    }
                }
                catch (Exception e)
                {
                    FoxyLogger.LogException(e);
                    ResetAllSettings();
                }
            }
            else
            {
                ResetAllSettings();
                if (m_verboseLogging)
                {
                    FoxyLogger.Log($"File {SettingsFilePath} does not exist. Initializing new file...");
                }
            }
            Localization.SetLanguage(LanguageIndex);
            Screen.SetResolution(ScreenWidth, ScreenHeight, IsFullscreen);
        }

        private void Update()
        {
            if (m_isSettingsDirty)
            {
                File.WriteAllText(SettingsFilePath, JsonUtility.ToJson(m_currentSettings, m_writeFileWithPrettyFormat));
                m_isSettingsDirty = false;
                if (m_verboseLogging)
                {
                    FoxyLogger.Log($"File written to {SettingsFilePath}.");
                }
            }
        }
    }
}
