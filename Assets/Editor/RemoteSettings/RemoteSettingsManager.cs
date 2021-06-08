#if UNITY_2018_3_OR_NEWER
#error Remote Settings Plugin is deprecated for Unity 2018.3+. The Analytics Package should be used. Install using the Unity Package Manager.
#else
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine.Networking;

namespace UnityEngine.Analytics
{
    [Serializable]
    public struct RemoteSettingsKeyValueType
    {
        public string key;
        public string value;
        public string type;
    }

    public class RemoteSettingsManager : EditorWindow
    {
        private readonly static string k_PluginVersion = "0.1.8";

        // EditorPrefs keys
        private readonly static string k_FetchKey = "UnityAnalyticsRemoteSettingsFetchKey";
        private readonly static string k_Installed = "UnityAnalyticsRemoteSettingsInstallKey";
        private readonly static string k_RSKeysExist = "UnityAnalyticsRemoteSettingsAreSet";
        private readonly static string k_CurrentEnvironment = "UnityAnalyticsRemoteSettingsEnvironment";

        //GUI variables
        private const string k_TabTitle = "Remote Settings";
        private const string k_ServerErrorDialogTitle = "Can't get Remote Settings";
        private const string k_ServerErrorDialogBtnLabel = "OK";
        private const string k_NoRSKeysError = "No RemoteSettings keys have been found, please go to the dashboard to add them.";

        //Labels
        private GUIContent m_SecretKeyContent = new GUIContent("Project Secret Key", "Copy the key from the 'Configure' page of your project dashboard");
        private GUIContent m_RemoteSettingsHeaderContent = new GUIContent("Unity Remote Settings");
        private GUIContent m_RemoteSettingsIntroContent = new GUIContent("Unity Remote Settings enables to make the game appearance and properties of your app without publishing an app update");
        private GUIContent m_SecretKeyPrefixContent = new GUIContent("Please enter the Project Secret Key to authenticate your project.");
        private GUIContent m_GridKeyContent = new GUIContent("Key");
        private GUIContent m_GridTypeContent = new GUIContent("Type");
        private GUIContent m_GridValueContent = new GUIContent("Value");
        private GUIContent m_RemoteSettingsSetupContent = new GUIContent("To start setting up key-value pairs for the Remote Settings");
        private GUIContent m_RefreshKeysContent = new GUIContent("If you already have key-value pairs in your dashboard");
        private GUIContent m_AnalyticsNotEnabledHeaderContent = new GUIContent("Unity Analytics is not enabled");
        private GUIContent m_AnalyticsNotEnabledContent = new GUIContent("To use Unity Remote Settings, please enable Unity Analytics from the Services window. Go to Window > Services to open Unity Services Window and follow the prompts.");

        //Link Labels
        private GUIContent m_LearnMoreLinkContent = new GUIContent("Learn more");
        private GUIContent m_SecretKeyLinkContent = new GUIContent("Look up the key.");

        //Button Labels
        private GUIContent m_NextButtonContent = new GUIContent("Next");
        private GUIContent m_GoToDashboardButtonContent = new GUIContent("Go To Dashboard");
        private GUIContent m_RefreshButtonContent = new GUIContent("Refresh");

        private Vector2 m_RemoteSettingsListScrollPos;

        private const float k_HeaderSpace = 10f;
        private const float k_AfterParagraphSpace = 20f;
        private const float k_ColumnCount = 3f;

        // Reflected to access internal method EditorGUILayout.LinkLabel
        private MethodInfo m_EditorGUILayoutLinkLabel;

        //File path vars
        private const string k_PathUnity = "Unity";
        private const string k_PathAnalytics = "Analytics";
        private const string k_PathConfig = "config";
        private const string k_DataStoreName = "RemoteSettingsDataStore";
        private const string k_PathToDataStore = "Assets/Editor/RemoteSettings/Data/{0}.asset";
        private const string k_RemoteSettingsDataPath = "Assets/Editor/RemoteSettings/Data";

        //Web variables
        //REST API paths
        private const string k_BasePath = "https://analytics.cloud.unity3d.com/";
        private const string k_APIPath = k_BasePath + "api/v2/projects/";
        private const string k_ConfigurationPath = k_APIPath + "{0}/configurations/";
        private const string k_RemoteSettingsPath = k_APIPath + "{0}/configurations/{1}/remotesettings";

        //Link URLs
        private const string k_DocumentationURL = "https://docs.unity3d.com/Manual/UnityAnalyticsRemoteSettings.html";
        private const string k_SecretKeyURL = k_BasePath + "projects/{0}/edit/";
        private const string k_DashboardURL = k_BasePath + "remote_settings/{0}/";

        // Actual state
        private string m_AppId = "";
        private string m_SecretKey = "";
        private RemoteSettingsHolder RSDataStore;
        private string m_CurrentEnvironment = "Release";
        private List<ApplicationConfig> m_Configurations = new List<ApplicationConfig>();

        private IEnumerator<AsyncOperation> m_WebRequestEnumerator;


        [Serializable]
        private struct RemoteSettingsData
        {
            public List<RemoteSettingsKeyValueType> list;
        }

        [Serializable]
        private struct ApplicationConfig
        {
            public string id;
            public string appId;
            public string name;
            public string description;
            public string createdAt; // DateTime?
            public string updatedAt;
            public string contentHash;
        }

        [Serializable]
        private struct ApplicationConfigs
        {
            public List<ApplicationConfig> list;
        }

        [MenuItem("Window/Unity Analytics/Remote Settings")]
        static void RemoteSettingsManagerMenuOption()
        {
            EditorWindow.GetWindow(typeof(RemoteSettingsManager), false, k_TabTitle);
        }

        private void OnEnable()
        {
            // Reflect out the internal API EditorGUILayout.LinkLabel
            var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var parameters = new Type[] { typeof(GUIContent), typeof(GUILayoutOption[]) };
            m_EditorGUILayoutLinkLabel = typeof(EditorGUILayout).GetMethod("LinkLabel", flags, null, parameters, null);

            if (UnityEditor.Analytics.AnalyticsSettings.enabled)
            {
                if (EditorPrefs.GetBool(k_Installed + m_AppId))
                {
                    CheckAndCreateDataStore();
                    SubmitRequest();
                    this.Repaint();
                }
            }

            EditorApplication.update += Update;
        }

        void OnDisable()
        {
            EditorApplication.update -= Update;
            m_WebRequestEnumerator = null;
        }

        void OnFocus()
        {
            if (EditorPrefs.GetBool(k_Installed + m_AppId, false))
            {
                RestoreValues();
            }
            else
            {
                SetInitValues();
            }
            if (RSDataStore == null)
            {
                CheckAndCreateDataStore();
            }
        }

        protected void RestoreAppId()
        {
            #if UNITY_5_3_OR_NEWER
            if (string.IsNullOrEmpty(m_AppId) && !string.IsNullOrEmpty(Application.cloudProjectId) || !m_AppId.Equals(Application.cloudProjectId))
            {
                m_AppId = Application.cloudProjectId;
            }
            #endif
        }

        protected void SetInitValues()
        {
            RestoreAppId();
            if (!string.IsNullOrEmpty(EditorPrefs.GetString(k_FetchKey + m_AppId)))
            {
                m_SecretKey = EditorPrefs.GetString(k_FetchKey + m_AppId, m_SecretKey);
            }
        }

        protected void RestoreValues()
        {
            RestoreAppId();

            m_SecretKey = EditorPrefs.GetString(k_FetchKey + m_AppId, m_SecretKey);

            m_CurrentEnvironment = EditorPrefs.GetString(k_CurrentEnvironment + m_AppId, m_CurrentEnvironment);
        }

        private void CheckAndCreateDataStore()
        {
            string formattedPath = string.Format(k_PathToDataStore, k_DataStoreName);
            if (AssetDatabase.FindAssets(k_DataStoreName).Length == 0)
            {
                RemoteSettingsHolder asset = ScriptableObject.CreateInstance<RemoteSettingsHolder>();
                asset.rsKeyList = new List<RemoteSettingsKeyValueType>();
                CheckAndCreateAssetFolder();
                AssetDatabase.CreateAsset(asset, formattedPath);
                AssetDatabase.SaveAssets();
                RSDataStore = AssetDatabase.LoadAssetAtPath(formattedPath, typeof(RemoteSettingsHolder)) as RemoteSettingsHolder;
            }
            else
            {
                RSDataStore = AssetDatabase.LoadAssetAtPath(formattedPath, typeof(RemoteSettingsHolder)) as RemoteSettingsHolder;
            }

            CreateDataStoreDict();
        }

        private void CreateDataStoreDict()
        {
            if (RSDataStore && RSDataStore.rsKeyList.Count != 0)
            {
                if (RSDataStore.rsKeys == null)
                {
                    RSDataStore.rsKeys = new SortedDictionary<string, RemoteSettingsKeyValueType>();
                }
                else
                {
                    RSDataStore.rsKeys.Clear();
                }
                foreach (RemoteSettingsKeyValueType rsPair in RSDataStore.rsKeyList)
                {
                    RSDataStore.rsKeys.Add(rsPair.key, rsPair);
                }
            }
        }

        private void OnGUI()
        {
            if (UnityEditor.Analytics.AnalyticsSettings.enabled)
            {
                if (!EditorPrefs.GetBool(k_Installed + m_AppId))
                {
                    AddHeader();
                    RestoreAppId();
                    GUILayout.Label(m_SecretKeyPrefixContent, EditorStyles.wordWrappedLabel);

                    //if (EditorGUILayout.LinkLabel(m_SecretKeyLinkContent)) // LinkLabel is internal
                    if ((bool)m_EditorGUILayoutLinkLabel.Invoke(null, new object[] { m_SecretKeyLinkContent, null }))
                    {
                        Application.OpenURL(string.Format(k_SecretKeyURL, m_AppId));
                    }

                    string oldKey = m_SecretKey;
                    m_SecretKey = EditorGUILayout.TextField(m_SecretKeyContent, m_SecretKey);
                    if (oldKey != m_SecretKey && !string.IsNullOrEmpty(m_SecretKey))
                    {
                        EditorPrefs.SetString(k_FetchKey + m_AppId, m_SecretKey);
                    }
                    GUI.enabled = !string.IsNullOrEmpty(m_SecretKey);
                    GUILayout.Space(k_AfterParagraphSpace);
                    if (GUILayout.Button(m_NextButtonContent))
                    {
                        CheckAndCreateDataStore();
                        SubmitRequest();
                        if (GUI.changed)
                        {
                            EditorUtility.SetDirty(RSDataStore);
                        }
                    }
                    GUI.enabled = true;
                }
                else if (EditorPrefs.GetBool(k_Installed + m_AppId) && EditorPrefs.GetBool(k_RSKeysExist + m_AppId))
                {
                    EditorGUI.BeginDisabledGroup(m_Configurations.Count <= 1);
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Configuration");
                        GUIContent ddBtnContent = new GUIContent(m_CurrentEnvironment);
                        Rect rect = GUILayoutUtility.GetRect(ddBtnContent, EditorStyles.popup);
                        if (GUI.Button(rect, ddBtnContent, EditorStyles.popup))
                        {
                            BuildPopupListForRSEnvironments().DropDown(rect);
                        }
                    }
                    EditorGUI.EndDisabledGroup();

                    float columnWidth = EditorGUIUtility.currentViewWidth / k_ColumnCount;
                    using (new GUILayout.VerticalScope("box"))
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new GUILayout.HorizontalScope("box"))
                            {
                                GUILayout.Label(m_GridKeyContent, EditorStyles.boldLabel, GUILayout.Width(columnWidth));
                                GUILayout.Label(m_GridTypeContent, EditorStyles.boldLabel, GUILayout.Width(columnWidth));
                                GUILayout.Label(m_GridValueContent, EditorStyles.boldLabel);
                            }
                        }
                        using (new GUILayout.VerticalScope("box"))
                        {
                            if (RSDataStore.rsKeys != null)
                            {
                                m_RemoteSettingsListScrollPos = EditorGUILayout.BeginScrollView(m_RemoteSettingsListScrollPos);
                                foreach (KeyValuePair<string, RemoteSettingsKeyValueType> rsPair in RSDataStore.rsKeys)
                                {
                                    using (new EditorGUILayout.HorizontalScope())
                                    {
                                        GUILayout.Label(rsPair.Key, GUILayout.Width(columnWidth));
                                        GUILayout.Label(rsPair.Value.type, GUILayout.Width(columnWidth));
                                        GUILayout.Label(rsPair.Value.value, EditorStyles.wordWrappedLabel);
                                    }
                                }
                                EditorGUILayout.EndScrollView();
                            }
                        }
                    }
                    AddFooterButtons();
                }
                else if (EditorPrefs.GetBool(k_Installed + m_AppId) && !EditorPrefs.GetBool(k_RSKeysExist + m_AppId))
                {
                    AddHeader();
                    GUILayout.Label(m_RemoteSettingsSetupContent);
                    if (GUILayout.Button(m_GoToDashboardButtonContent, GUILayout.Width(EditorGUIUtility.currentViewWidth / 2)))
                    {
                        Application.OpenURL(string.Format(k_DashboardURL, m_AppId));
                    }
                    GUILayout.Space(k_AfterParagraphSpace);
                    GUILayout.Label(m_RefreshKeysContent);
                    if (GUILayout.Button(m_RefreshButtonContent, GUILayout.Width(EditorGUIUtility.currentViewWidth / 2)))
                    {
                        SubmitRequest(true);
                    }
                }
            }
            else
            {
                AddHeader();
                GUILayout.Label(m_AnalyticsNotEnabledHeaderContent, EditorStyles.boldLabel);
                GUILayout.Label(m_AnalyticsNotEnabledContent, EditorStyles.wordWrappedLabel);
            }
            if (GUI.changed)
            {
                EditorUtility.SetDirty(RSDataStore);
            }
        }

        private GenericMenu BuildPopupListForRSEnvironments()
        {
            var menu = new GenericMenu();

            for (int i = 0; i < m_Configurations.Count; i++)
            {
                string name = m_Configurations[i].name;
                menu.AddItem(new GUIContent(name), name == m_CurrentEnvironment, EnvironmentSelectionCallback, name);
            }

            return menu;
        }

        private void EnvironmentSelectionCallback(object obj)
        {
            var envrionmentName = (string)obj;
            SetCurrentEnvironment(envrionmentName);
            SubmitRequest(true);
        }

        void SetCurrentEnvironment(string envrionmentName)
        {
            m_CurrentEnvironment = envrionmentName;
            EditorPrefs.SetString(k_CurrentEnvironment + m_AppId, envrionmentName);
        }

        private void AddHeader()
        {
            GUILayout.Space(k_HeaderSpace);
            GUILayout.Label(m_RemoteSettingsHeaderContent, EditorStyles.boldLabel);
            GUILayout.Label(m_RemoteSettingsIntroContent, EditorStyles.wordWrappedLabel);

            //if (EditorGUILayout.LinkLabel(m_LearnMoreLinkContent)) // LinkLabel is internal
            if ((bool)m_EditorGUILayoutLinkLabel.Invoke(null, new object[] { m_LearnMoreLinkContent, null }))
            {
                Application.OpenURL(k_DocumentationURL);
            }

            GUILayout.Space(k_AfterParagraphSpace);
        }

        private void AddFooterButtons()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(m_RefreshButtonContent, GUILayout.Width(EditorGUIUtility.currentViewWidth / 2)))
                {
                    string filePath = Path.Combine(Application.persistentDataPath, k_PathUnity);
                    filePath = Path.Combine(filePath, m_AppId);
                    filePath = Path.Combine(filePath, k_PathAnalytics);
                    filePath = Path.Combine(filePath, k_PathConfig);
                    FileUtil.DeleteFileOrDirectory(filePath);
                    SubmitRequest(true);
                }
                if (GUILayout.Button(m_GoToDashboardButtonContent))
                {
                    Application.OpenURL(string.Format(k_DashboardURL, m_AppId));
                }
            }
        }

        private void CheckAndCreateAssetFolder()
        {
            string[] folders = k_RemoteSettingsDataPath.Split('/');
            string assetPath = null;
            foreach (string folder in folders)
            {
                if (assetPath == null)
                {
                    assetPath = folder;
                }
                else
                {
                    string folderPath = System.IO.Path.Combine(assetPath, folder);
                    if (!Directory.Exists(folderPath))
                    {
                        AssetDatabase.CreateFolder(assetPath, folder);
                    }
                    assetPath = folderPath;
                }
            }
        }

        IEnumerator<AsyncOperation> FetchRemoteSettings(string url, bool explicitlyRequested)
        {
            var configRequest = Authorize(UnityWebRequest.Get(url));
#if UNITY_2017_2_OR_NEWER
            yield return configRequest.SendWebRequest();
#else
            yield return configRequest.Send();
#endif

#if UNITY_2017_1_OR_NEWER
            if (configRequest.isNetworkError || configRequest.isHttpError)
#else
            if (configRequest.isError || configRequest.responseCode >= 400)
#endif
            {
                Debug.LogWarning("Failed to fetch remote settings configurations: " + configRequest.error);
                yield break;
            }

            var configJson = configRequest.downloadHandler.text;
            var currentId = LoadConfigurations(configJson, explicitlyRequested);

            if (currentId == "")
            {
                yield break;
            }

            string remoteSettingsUrl = string.Format(k_RemoteSettingsPath, m_AppId, currentId);
            var settingsRequest = Authorize(UnityWebRequest.Get(remoteSettingsUrl));

#if UNITY_2017_2_OR_NEWER
            yield return settingsRequest.SendWebRequest();
#else
            yield return settingsRequest.Send();
#endif

#if UNITY_2017_1_OR_NEWER
            if (settingsRequest.isNetworkError || settingsRequest.isHttpError)
#else
            if (settingsRequest.isError || settingsRequest.responseCode >= 400)
#endif
            {
                Debug.LogWarning("Failed to fetch remote settings: " + settingsRequest.error);
                yield break;
            }

            string remoteSettingsJson = settingsRequest.downloadHandler.text;
            LoadRemoteSettings(remoteSettingsJson);
        }

        private void SubmitRequest(bool explicitlyRequested = false)
        {
            string url = string.Format(k_ConfigurationPath, m_AppId);
            m_WebRequestEnumerator = FetchRemoteSettings(url, explicitlyRequested);
        }

        void Update()
        {
            UpdateCoroutine();
        }

        void UpdateCoroutine()
        {
            if (m_WebRequestEnumerator != null)
            {
                if (m_WebRequestEnumerator.Current == null)
                {
                    m_WebRequestEnumerator.MoveNext();
                }
                if (m_WebRequestEnumerator.Current.isDone && !m_WebRequestEnumerator.MoveNext())
                {
                    m_WebRequestEnumerator = null;
                }
            }
        }

        // Returns the id of the currently active configuration
        // If the currently active configuration doesn't exist, this will choose an existing one.
        string LoadConfigurations(string configsResult, bool explicitlyRequested)
        {
            string configsJson = "{ \"list\": " + configsResult + "}";

            try
            {
                m_Configurations = JsonUtility.FromJson<ApplicationConfigs>(configsJson).list;
            }
            catch (Exception e)
            {
                Debug.LogWarning("Remote Settings configuration response was not valid JSON:\n" + configsResult + "\n" + e);
                return "";
            }

            EditorPrefs.SetBool(k_Installed + m_AppId, true);

            if (m_Configurations.Count == 0)
            {
                EditorPrefs.SetBool(k_RSKeysExist + m_AppId, false);
                if (explicitlyRequested)
                {
                    // TODO: This should display text in the window with a link, instead of a dialog
                    EditorUtility.DisplayDialog(k_ServerErrorDialogTitle, k_NoRSKeysError, k_ServerErrorDialogBtnLabel);
                }

                return "";
            }

            // Get a config (preferably the selected one)
            var currentConfig = m_Configurations[0];  // Default to the first existing one
            foreach (var appConfig in m_Configurations)
            {
                if (appConfig.name == m_CurrentEnvironment)
                {
                    currentConfig = appConfig;
                    break;
                }
            }

            SetCurrentEnvironment(currentConfig.name);

            return currentConfig.id;
        }

        void LoadRemoteSettings(string remoteSettingsResult)
        {
            string remoteSettingsJson = "{ \"list\": " + remoteSettingsResult + "}";

            List<RemoteSettingsKeyValueType> remoteSettings;
            try
            {
                remoteSettings = JsonUtility.FromJson<RemoteSettingsData>(remoteSettingsJson).list;
            }
            catch (Exception e)
            {
                Debug.LogWarning("Remote Settings response was not valid JSON:\n" + remoteSettingsResult + "\n" + e);
                return;
            }

            RSDataStore.rsKeyList.Clear();
            if (remoteSettings.Count == 0)
            {
                EditorPrefs.SetBool(k_RSKeysExist + m_AppId, false);
            }
            else
            {
                foreach (var rs in remoteSettings)
                {
                    RSDataStore.rsKeyList.Add(rs);
                }

                EditorPrefs.SetBool(k_RSKeysExist + m_AppId, true);
            }

            CreateDataStoreDict();
        }

        private UnityWebRequest Authorize(UnityWebRequest request)
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("User-Agent", "Unity Editor " + Application.unityVersion + " RS " + k_PluginVersion);
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(m_AppId + ":" + m_SecretKey));
            request.SetRequestHeader("Authorization", string.Format("Basic {0}", credentials));
            return request;
        }
    }
}
#endif
