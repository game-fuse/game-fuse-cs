using UnityEditor;
using UnityEngine;
using GameFuse.Config;

namespace GameFuse.Editor
{
    /// <summary>
    /// Custom editor for GameFuseSettings ScriptableObject.
    /// Provides enhanced UI and validation for GameFuse settings.
    /// </summary>
    [CustomEditor(typeof(GameFuseSettings))]
    public class GameFuseSettingsEditor : UnityEditor.Editor
    {
        private SerializedProperty _gameId;
        private SerializedProperty _gameApiKey;
        private SerializedProperty _apiBaseUrl;
        private SerializedProperty _maxRetryAttempts;
        private SerializedProperty _requestTimeoutSeconds;

        private void OnEnable()
        {
            _gameId = serializedObject.FindProperty("GameId");
            _gameApiKey = serializedObject.FindProperty("GameApiKey");
            _apiBaseUrl = serializedObject.FindProperty("ApiBaseUrl");
            _maxRetryAttempts = serializedObject.FindProperty("MaxRetryAttempts");
            _requestTimeoutSeconds = serializedObject.FindProperty("RequestTimeoutSeconds");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("GameFuse Configuration", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_gameId);
            EditorGUILayout.PropertyField(_gameApiKey);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Advanced Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_apiBaseUrl);
            EditorGUILayout.PropertyField(_maxRetryAttempts);
            EditorGUILayout.PropertyField(_requestTimeoutSeconds);

            DrawHelpBox();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHelpBox()
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "These settings are required for the GameFuse SDK to function properly.\n\n" +
                "Game ID and Game API Key can be found in your GameFuse dashboard.\n\n" +
                "After configuring these settings, ensure this asset is placed in a Resources folder in your project.",
                MessageType.Info);

            GameFuseSettings settings = (GameFuseSettings)target;

            if (string.IsNullOrEmpty(settings.GameId))
            {
                EditorGUILayout.HelpBox("Game ID is required.", MessageType.Warning);
            }

            if (string.IsNullOrEmpty(settings.GameApiKey))
            {
                EditorGUILayout.HelpBox("Game API Key is required.", MessageType.Warning);
            }

            // Check if the asset is in a Resources folder
            string assetPath = AssetDatabase.GetAssetPath(target);
            if (!assetPath.Contains("/Resources/"))
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(
                    "This asset should be placed in a Resources folder to be accessible at runtime.\n\n" +
                    "Current path: " + assetPath,
                    MessageType.Warning);

                if (GUILayout.Button("Create Resources Folder and Move Asset"))
                {
                    MoveAssetToResourcesFolder(assetPath);
                }
            }
        }

        private void MoveAssetToResourcesFolder(string currentPath)
        {
            string directory = System.IO.Path.GetDirectoryName(currentPath);
            string resourcesPath = System.IO.Path.Combine(directory, "Resources");
            string fileName = System.IO.Path.GetFileName(currentPath);
            string newPath = System.IO.Path.Combine(resourcesPath, fileName);

            // Create Resources folder if it doesn't exist
            if (!System.IO.Directory.Exists(resourcesPath))
            {
                System.IO.Directory.CreateDirectory(resourcesPath);
                AssetDatabase.Refresh();
            }

            // Move the asset
            AssetDatabase.MoveAsset(currentPath, newPath);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Creates a new GameFuseSettings asset in the project.
        /// </summary>
        [MenuItem("Assets/Create/GameFuse/Settings")]
        public static void CreateGameFuseSettings()
        {
            // Create the settings asset
            GameFuseSettings settings = CreateInstance<GameFuseSettings>();

            // Determine where to save the asset
            string directory = Selection.activeObject != null
                ? AssetDatabase.GetAssetPath(Selection.activeObject)
                : "Assets";

            if (!string.IsNullOrEmpty(directory) && !System.IO.Directory.Exists(directory))
            {
                directory = System.IO.Path.GetDirectoryName(directory);
            }

            if (string.IsNullOrEmpty(directory) || directory == "Assets")
            {
                directory = "Assets";
                // Check if a Resources folder exists at the root, if not create one
                if (!System.IO.Directory.Exists("Assets/Resources"))
                {
                    System.IO.Directory.CreateDirectory("Assets/Resources");
                    AssetDatabase.Refresh();
                }
                directory = "Assets/Resources";
            }
            else
            {
                // Check if there's a Resources folder in the selected path or its parents
                bool hasResourcesFolder = false;
                string tempDir = directory;
                while (!string.IsNullOrEmpty(tempDir) && tempDir != "Assets")
                {
                    if (tempDir.EndsWith("/Resources"))
                    {
                        hasResourcesFolder = true;
                        directory = tempDir;
                        break;
                    }
                    tempDir = System.IO.Path.GetDirectoryName(tempDir);
                }

                // If no Resources folder found, create one in the selected directory
                if (!hasResourcesFolder)
                {
                    directory = System.IO.Path.Combine(directory, "Resources");
                    if (!System.IO.Directory.Exists(directory))
                    {
                        System.IO.Directory.CreateDirectory(directory);
                        AssetDatabase.Refresh();
                    }
                }
            }

            // Create the asset
            string assetPath = System.IO.Path.Combine(directory, "GameFuseSettings.asset");
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Select and ping the created asset
            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);
        }
    }
}