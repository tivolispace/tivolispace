using UnityEditor;
using UnityEditor.SceneManagement;

namespace Tivoli.Editor
{
    [InitializeOnLoad]
    public static class LoadInitializeSceneOnPlay
    {
        static LoadInitializeSceneOnPlay()
        {
            var pathOfFirstScene = EditorBuildSettings.scenes[0].path;
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
            EditorSceneManager.playModeStartScene = sceneAsset;
        }
    }
}