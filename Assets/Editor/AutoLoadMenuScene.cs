#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class AutoLoadMenuScene
{
    static AutoLoadMenuScene()
    {
        SceneAsset menuScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Menu.unity");
        if (menuScene != null)
        {
            EditorSceneManager.playModeStartScene = menuScene;
        }
    }

    [MenuItem("Dragon Ball/Mở Menu Scene")]
    public static void OpenMenuScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity");
        }
    }
}
#endif
