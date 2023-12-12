using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace UrbanFox
{
    public static class Localization
    {
        public const string LanguageXMLAssetResourceFolder = "Languages";

        private static Dictionary<string, string>[] _fullLocalizationData = null;
        private static readonly List<string> _missingKeys = new List<string>();
        private static event Action _onLanguageChanged;

        public static Dictionary<string, string> CurrentLanguageData => CurrentLanguageIndex.IsInRange(_fullLocalizationData) ? _fullLocalizationData[CurrentLanguageIndex] : null;
        public static int NumberOfLanguages => _fullLocalizationData.IsNullOrEmpty() ? 0 : _fullLocalizationData.Count();
        public static List<string> Keys { get; private set; }
        public static int CurrentLanguageIndex { get; private set; }
        public static string LanguageKeyName { get; private set; }
        public static bool IsInitialized { get; private set; }

        public static event Action OnLanguageChanged
        {
            add
            {
                _onLanguageChanged += value;
                value?.Invoke();
            }
            remove
            {
                _onLanguageChanged -= value;
            }
        }

        static Localization()
        {
            LoadLanguageXMLAssets();
        }

        public static bool TryGetLocalization(this string key, out string value)
        {
            if (CurrentLanguageData != null)
            {
                if (CurrentLanguageData.ContainsKey(key))
                {
                    value = CurrentLanguageData[key];
                    return true;
                }
            }
            _missingKeys.Add(key);
            value = $"!!{key}";
            return false;
        }

        public static bool TryGetLocalizationOverride(this string key, int overrideLanguageIndex, out string value)
        {
            if (overrideLanguageIndex.IsInRange(_fullLocalizationData))
            {
                var overrideLanguageData = _fullLocalizationData[overrideLanguageIndex];
                if (overrideLanguageData != null && overrideLanguageData.ContainsKey(key))
                {
                    value = overrideLanguageData[key];
                    return true;
                }
            }
            _missingKeys.Add(key);
            value = $"!!{key}";
            return false;
        }

        public static bool TryGetLocalizationOverride(this string key, string overrideLanguageName, out string value)
        {
            if (!_fullLocalizationData.IsNullOrEmpty())
            {
                for (int i = 0; i < _fullLocalizationData.Length; i++)
                {
                    var data = _fullLocalizationData[i];
                    if (data != null && data.ContainsKey(LanguageKeyName) && data[LanguageKeyName] == overrideLanguageName)
                    {
                        value = data[key];
                        return true;
                    }
                }
            }
            _missingKeys.Add(key);
            value = $"!!{key}";
            return false;
        }

        public static string GetLocalization(this string key)
        {
            key.TryGetLocalization(out var value);
            return value;
        }

        public static string GetLocalizationOverride(this string key, int overrideLanguageIndex)
        {
            key.TryGetLocalizationOverride(overrideLanguageIndex, out var value);
            return value;
        }

        public static string GetLocalizationOverride(this string key, string overrideLanguageName)
        {
            key.TryGetLocalizationOverride(overrideLanguageName, out var value);
            return value;
        }

        public static string GetLanguageCodeNameByLanguageIndex(int languageIndex)
        {
            return languageIndex.IsInRange(_fullLocalizationData) ? _fullLocalizationData[languageIndex].First().Value : string.Empty;
        }

        public static void SetLanguage(int languageIndex)
        {
            if (languageIndex.IsInRange(_fullLocalizationData))
            {
                CurrentLanguageIndex = languageIndex;
                _onLanguageChanged?.Invoke();
            }
        }

        public static void SetLanguage(string languageName)
        {
            if (!_fullLocalizationData.IsNullOrEmpty())
            {
                for (int i = 0; i < _fullLocalizationData.Length; i++)
                {
                    var data = _fullLocalizationData[i];
                    if (data != null && data.ContainsKey(LanguageKeyName) && data[LanguageKeyName] == languageName)
                    {
                        SetLanguage(i);
                        return;
                    }
                }
            }
        }

        public static void ShiftLanguageRight()
        {
            if (!_fullLocalizationData.IsNullOrEmpty())
            {
                var nextIndex = CurrentLanguageIndex + 1;
                if (nextIndex >= _fullLocalizationData.Length)
                {
                    nextIndex = 0;
                }
                SetLanguage(nextIndex);
            }
        }

        public static void ShiftLanguageLeft()
        {
            if (!_fullLocalizationData.IsNullOrEmpty())
            {
                var nextIndex = CurrentLanguageIndex - 1;
                if (nextIndex < 0)
                {
                    nextIndex = _fullLocalizationData.Length - 1;
                }
                SetLanguage(nextIndex);
            }
        }

        public static void PrintMissingKeys()
        {
            if (!_missingKeys.IsNullOrEmpty())
            {
                FoxyLogger.LogError($"{_missingKeys.Count} missing keys found:\n - {string.Join("\n - ", _missingKeys)}");
            }
        }

        public static void LoadLanguageXMLAssets()
        {
            LanguageKeyName = null;
            _fullLocalizationData = null;
            _missingKeys?.Clear();
            IsInitialized = false;

            var languageAssets = Resources.LoadAll<TextAsset>(LanguageXMLAssetResourceFolder);

            if (languageAssets.IsNullOrEmpty())
            {
                FoxyLogger.LogWarning($"Language XML files cannot be found. Please move or download language files in Resource's \"{LanguageXMLAssetResourceFolder}\" folder.");
                return;
            }

            _fullLocalizationData = new Dictionary<string, string>[languageAssets.Length];
            try
            {
                for (int i = 0; i < languageAssets.Length; i++)
                {
                    var xmlDocument = new XmlDocument();
                    try
                    {
                        xmlDocument.LoadXml(languageAssets[i].text);

                        var root = xmlDocument.DocumentElement;
                        var nodeList = root.SelectNodes("descendant::*");
                        var thisDictionary = new Dictionary<string, string>();

                        for (int j = 0; j < nodeList.Count; j++)
                        {
                            // Skip blank keys:
                            if (string.IsNullOrWhiteSpace(nodeList[j].Attributes["k"].Value))
                            {
                                continue;
                            }

                            // Skip duplicated keys:
                            if (thisDictionary.ContainsKey(nodeList[j].Attributes["k"].Value))
                            {
                                FoxyLogger.LogWarning($"Duplicated key exists: {nodeList[j].Attributes["k"]}");
                                continue;
                            }

                            thisDictionary.Add(nodeList[j].Attributes["k"].Value.FromXMLAttribute(), nodeList[j].Attributes["v"].Value.FromXMLAttribute());

                            // The key name is usually the 0th cell content:
                            if (i == 0 && j == 0)
                            {
                                LanguageKeyName = nodeList[j].Attributes["k"].Value;
                            }
                        }

                        if (i == 0)
                        {
                            Keys = thisDictionary.Keys.ToList();
                        }

                        _fullLocalizationData[i] = thisDictionary;
                    }
                    catch (Exception exception)
                    {
                        FoxyLogger.LogError($"Cannot load XML file: {languageAssets[i].name}.");
                        FoxyLogger.LogException(exception);
                    }
                }
            }
            catch (Exception exception)
            {
                FoxyLogger.LogError($"Error exists when fetching language text documents.");
                FoxyLogger.LogException(exception);
            }

            IsInitialized = true;
            _onLanguageChanged?.Invoke();
        }
    }
}
