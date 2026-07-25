using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class PlayFromHomeScene
{
    private const string HOME_SCENE_PATH = "Assets/Scenes/HomeScene.unity";

    static PlayFromHomeScene()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            if (EditorSceneManager.GetActiveScene().path != HOME_SCENE_PATH)
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(HOME_SCENE_PATH);
                }
                else
                {
                    EditorApplication.isPlaying = false;
                }
            }
        }
    }
}
