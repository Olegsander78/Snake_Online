#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Game.Development
{
    public static class SceneMenu
    {
        //[MenuItem("Scene/Open Loading Scene")]
        //private static void OpenLoadingScene()
        //{
        //    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        //    {
        //        EditorSceneManager.OpenScene("Assets/Game/Scenes/AppScene.unity");
        //    }
        //}

        [MenuItem("Scene/Open Game Scene")]
        private static void OpenGameScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/Game/Scenes/GameScene.unity");
            }
        }
        
        [MenuItem("Tools/Play Game")]
        private static void Play()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                var name = EditorSceneManager.GetActiveScene().name;
                if (name != "GameScene")
                {
                    const string scenePath = "Assets/Game/Scenes/GameScene.unity";
                    EditorSceneManager.OpenScene(scenePath);
                }
                
                EditorApplication.isPlaying = true;
            }
        }
    }
}
#endif