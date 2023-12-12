using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

namespace UrbanFox.Editor
{
    public class LocalizationBrowser : EditorWindow
    {
        private const string _sourceSpreadsheetIDKey = "UrbanFox.Editor.LocalizationBrowser.SpreadsheetSourceKey.";
        private const string _assetCreationFolder = "Assets/Resources/" + Localization.LanguageXMLAssetResourceFolder;

        private const string _IDToken = "{ID}";
        private const string _sourceSpreadsheetURLOriginalFormat = "https://docs.google.com/spreadsheets/d/" + _IDToken;
        private const string _sourceSpreadsheetURLJsonFormat = "https://docs.google.com/spreadsheets/d/" + _IDToken + "/gviz/tq";

        private static Vector2 _scroll = default;

        [SerializeField] private string _sourceSpreadsheetID = string.Empty;
        [SerializeField] private string _searchKey = string.Empty;

        private UnityWebRequest _webRequest = null;
        private string _downloadedJSONText = string.Empty;
        private List<List<string>> _languageAndKeyData = null;
        private bool[] _visibleLanguages = null;

        private bool IsDownloading => _webRequest != null && _webRequest.result == UnityWebRequest.Result.InProgress;

        private bool[] VisibleLanguages
        {
            get
            {
                if (_visibleLanguages == null && Localization.IsInitialized)
                {
                    _visibleLanguages = new bool[Localization.NumberOfLanguages];
                    for (int i = 0; i < _visibleLanguages.Length; i++)
                    {
                        _visibleLanguages[i] = true;
                    }
                }
                return _visibleLanguages;
            }
        }

        private int VisibleLanguageCountPlusOne => VisibleLanguages.IsNullOrEmpty() ? 1 : VisibleLanguages.Where(x => x == true).ToList().Count + 1;
        private float InspectorWidth => EditorGUIUtility.currentViewWidth - 30;

        [MenuItem("OwO/Window/Localization Browser...")]
        private static void Init()
        {
            var window = GetWindow<LocalizationBrowser>();
            window.titleContent = new GUIContent("Localization Browser");
            window.minSize = new Vector2(500, 500);
            window.Show();
        }

        private void OnEnable()
        {
            _sourceSpreadsheetID = EditorPrefs.GetString(_sourceSpreadsheetIDKey + Application.productName, string.Empty);
        }

        private void OnDisable()
        {
            _webRequest?.Abort();
            _webRequest = null;
        }

        private void OnGUI()
        {
            GUILayout.Label("Localization Browser", EditorStyles.boldLabel);
            _sourceSpreadsheetID = EditorGUILayout.TextField("Spreadsheet ID", _sourceSpreadsheetID);

            EditorGUILayout.HelpBox($"Find the ID of a spreadsheet with this format: {_sourceSpreadsheetURLOriginalFormat}", MessageType.Info);

            EditorGUILayout.Space();

            GUILayout.Label("Data Operations (Requires Internet)", EditorStyles.boldLabel);
            using (var _ = new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Download", GUILayout.Height(40)))
                {
                    EditorPrefs.SetString(_sourceSpreadsheetIDKey + Application.productName, _sourceSpreadsheetID);
                    StartDownloadingData(_sourceSpreadsheetID);
                }

                var url = _sourceSpreadsheetURLOriginalFormat.Replace(_IDToken, _sourceSpreadsheetID);
                if (GUILayout.Button(new GUIContent("View on Google Sheets with Internet Browser...", url), GUILayout.Height(40)))
                {
                    FoxyLogger.Log($"Opening URL: {url}");
                    Application.OpenURL(url);
                }
            }

            GUILayoutExtensions.HorizontalLine();

            if (IsDownloading)
            {
                EditorGUILayout.HelpBox($"Downloading...", MessageType.Info);
            }
            else if (Localization.IsInitialized)
            {
                DrawLanguageSelector();
                EditorGUILayout.Space();
                DrawLanguageTable();
            }
            else
            {
                EditorGUILayout.HelpBox($"Localization not correctly initialized. Please check for missing language data.", MessageType.Warning);
            }
        }

        private void DrawLanguageSelector()
        {
            GUILayout.Label("Preview Languages", EditorStyles.boldLabel);
            for (int i = 0; i < Localization.NumberOfLanguages; i++)
            {
                if (i % 5 == 0)
                {
                    GUILayout.BeginHorizontal();
                }
                var buttonLabel = Localization.LanguageKeyName.GetLocalizationOverride(i);
                var isVisible = i.IsInRange(VisibleLanguages) ? VisibleLanguages[i] : false;
                if (GUILayoutExtensions.ColoredButton(buttonLabel, isVisible ? Color.yellow : Color.white, GUILayout.Width(InspectorWidth / 5)))
                {
                    VisibleLanguages[i] = !VisibleLanguages[i];
                }
                if (i % 5 == 4 || i == Localization.NumberOfLanguages - 1)
                {
                    GUILayout.EndHorizontal();
                }
            }
        }

        private void DrawLanguageTable()
        {
            var labelWidth = InspectorWidth / VisibleLanguageCountPlusOne - VisibleLanguageCountPlusOne * 0.25f;
            GUILayout.Label("Localization Data", EditorStyles.boldLabel);

            _searchKey = EditorGUILayoutExtensions.SearchText("Search Key", _searchKey);
            var cacheSearchText = _searchKey.ToLower();

            EditorGUILayout.Space();

            using (var _ = new GUILayout.HorizontalScope())
            {
                GUILayout.Label(Localization.LanguageKeyName, EditorStyles.boldLabel, GUILayout.Width(labelWidth));
                for (int i = 0; i < Localization.NumberOfLanguages; i++)
                {
                    if (i.IsInRange(VisibleLanguages) && VisibleLanguages[i])
                    {
                        GUILayout.Label(Localization.LanguageKeyName.GetLocalizationOverride(i), GUILayout.Width(labelWidth));
                    }
                }
            }

            _scroll = GUILayout.BeginScrollView(_scroll);
            for (int i = 0; i < Localization.Keys.Count; i++)
            {
                var key = Localization.Keys[i];
                if (key.ToLower().Contains(cacheSearchText))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(key, GUILayout.Width(labelWidth - 35));
                    if (GUILayout.Button(EditorIcons.DuplicateIcon, GUILayout.Width(30)))
                    {
                        EditorGUIUtility.systemCopyBuffer = key;
                        FoxyLogger.Log($"Key copied to system buffer: {key}");
                    }
                    GUI.enabled = false;
                    for (int j = 0; j < Localization.NumberOfLanguages; j++)
                    {
                        if (j.IsInRange(VisibleLanguages) && VisibleLanguages[j])
                        {
                            EditorGUILayout.TextField(key.GetLocalizationOverride(j), GUILayout.Width(labelWidth));
                        }
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
        }

        #region Functions for downloading and parsing JSON text from Google Sheets
        private void StartDownloadingData(string ID)
        {
            _webRequest = UnityWebRequest.Get(_sourceSpreadsheetURLJsonFormat.Replace(_IDToken, ID));
            _webRequest.SendWebRequest();
            EditorApplication.update += DownloadDataInProgress;
        }

        private void DownloadDataInProgress()
        {
            if (_webRequest == null || !_webRequest.isDone || _webRequest.result == UnityWebRequest.Result.InProgress)
            {
                return;
            }

            if (_webRequest.result == UnityWebRequest.Result.Success)
            {
                _downloadedJSONText = FetchTextInParentheses(_webRequest.downloadHandler.text);
                if (TryParseJSONTextToLanguageAndKeyData(_downloadedJSONText, out _languageAndKeyData))
                {
                    WriteLanguageAndKeyDataToXMLFiles(_languageAndKeyData);
                    Localization.LoadLanguageXMLAssets();
                }
                else
                {
                    FoxyLogger.LogError("Downloaded text json file contains invalid format. Cannot create language and key data.");
                }
            }
            else
            {
                FoxyLogger.LogError($"Download failed. Error message: {_webRequest.error}");
            }

            EditorApplication.update -= DownloadDataInProgress;
        }

        private string FetchTextInParentheses(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            var firstOpenedParenthesisIndex = source.IndexOf('(');
            var lastClosedParenthesisIndex = source.LastIndexOf(')');

            // If any parenthesis cannot be found, or if the opened parenthesis appears after the closed parenthesis: return empty.
            if (firstOpenedParenthesisIndex < 0 || lastClosedParenthesisIndex < 0 || firstOpenedParenthesisIndex > lastClosedParenthesisIndex)
            {
                return string.Empty;
            }

            return source.Substring(firstOpenedParenthesisIndex + 1, lastClosedParenthesisIndex - firstOpenedParenthesisIndex - 1);
        }

        private bool TryParseJSONTextToLanguageAndKeyData(string originalText, out List<List<string>> languageAndKeyData)
        {
            languageAndKeyData = null;
            var jObject = (JObject)JsonConvert.DeserializeObject(originalText);

            if (!jObject.ContainsKey("table"))
            {
                LogMissingKeyFromDownloadedText("table");
                return false;
            }
            var table = (JObject)jObject["table"];

            if (!table.ContainsKey("rows"))
            {
                LogMissingKeyFromDownloadedText("rows");
                return false;
            }
            var allRows = (JArray)table["rows"];

            // Used to count the number of cells (i.e. number of languages) in the 0th row. After that, this is set to true.
            bool languageCountSet = false;
            int languageCount = 0;

            // Temporary dictionary that will be cast to a nested list eventually.
            List<string>[] tempDictionary = null;

            foreach (var uncastRow in allRows)
            {
                if (uncastRow is not JObject row)
                {
                    FoxyLogger.LogError("Downloaded text parsing error. A null row exists.");
                    return false;
                }

                if (!row.ContainsKey("c"))
                {
                    LogMissingKeyFromDownloadedText("c");
                    return false;
                }

                int currentLanguageIndex = 0;
                var cellsOnSingleRow = (JArray)row["c"];
                foreach (var uncastCell in cellsOnSingleRow)
                {
                    if (uncastCell is JObject cell && cell.ContainsKey("v"))
                    {
                        if (languageCountSet && currentLanguageIndex.IsInRange(tempDictionary))
                        {
                            tempDictionary[currentLanguageIndex].Add(cell["v"].ToString());
                        }
                        else if (!languageCountSet && !string.IsNullOrWhiteSpace(cell["v"].ToString()))
                        {
                            languageCount++;
                        }
                    }
                    else
                    {
                        if (languageCountSet && currentLanguageIndex.IsInRange(tempDictionary))
                        {
                            // Add an empty string if a valid value not exists, in this case uncastCell is null.
                            tempDictionary[currentLanguageIndex].Add(string.Empty);
                        }
                    }
                    currentLanguageIndex++;
                }

                // Set up the temporary dictionary after counting the 0th row.
                if (!languageCountSet)
                {
                    languageCountSet = true;
                    tempDictionary = new List<string>[languageCount];
                    for (int x = 0; x < tempDictionary.Length; x++)
                    {
                        tempDictionary[x] = new List<string>();
                    }
                }
            }

            // Cast language data.
            languageAndKeyData = tempDictionary.Cast<List<string>>().ToList();

            return true;
        }
        #endregion

        private void WriteLanguageAndKeyDataToXMLFiles(List<List<string>> languageAndKeyData)
        {
            if (languageAndKeyData.IsNullOrEmpty() || languageAndKeyData.Count <= 1)
            {
                return;
            }

            if (!Directory.Exists(_assetCreationFolder))
            {
                Directory.CreateDirectory(_assetCreationFolder);
            }

            // The 0th list is the key
            var keys = languageAndKeyData[0];

            // Actual language keys start from the 1st list
            for (int i = 1; i < languageAndKeyData.Count; i++)
            {
                var content = new List<string> { "<?xml version=\"1.0\" encoding=\"UTF - 8\" standalone=\"yes\"?>", "<data>" };
                var languageData = languageAndKeyData[i];
                var languageName = languageData[0];
                for (int j = 0; j < languageData.Count; j++)
                {
                    content.Add($"    <i k=\"{keys[j].ToXMLAttribute()}\" v=\"{languageData[j].ToXMLAttribute()}\"/>");
                }
                content.Add("</data>");
                File.WriteAllLines(Path.Combine(_assetCreationFolder, $"{i}_{languageName}.xml"), content);
            }
            FoxyLogger.Log($"Successfully created {languageAndKeyData.Count - 1} language files.");
            AssetDatabase.Refresh();
        }

        private void LogMissingKeyFromDownloadedText(string keyName)
        {
            FoxyLogger.LogError($"Downloaded text is missing a requied key: {keyName}.");
        }
    }
}
