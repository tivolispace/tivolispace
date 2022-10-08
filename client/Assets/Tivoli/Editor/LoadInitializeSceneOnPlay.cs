using Tivoli.Scripts;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Tivoli.Editor
{
    [InitializeOnLoad]
    public static class LoadInitializeSceneOnPlay
    {
        static LoadInitializeSceneOnPlay()
        {
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