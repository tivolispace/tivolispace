using Tivoli.Scripts;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tivoli.Editor
{
    public static class LoadInitializeSceneOnPlay
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode) return;

            if (EditorPrefs.GetBool(TivoliEditorPrefs.OverridePlayMode, TivoliDefaultEditorPrefs.OverridePlayMode))
            {
                var pathOfFirstScene = EditorBuildSettings.scenes[0].path;
                var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
                EditorSceneManager.playModeStartScene = sceneAsset;
            }
            else
            {
                EditorSceneManager.playModeStartScene = null;
            }
        }
    }
}