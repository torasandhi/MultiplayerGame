//using UnityEngine;
//using UnityEditor;
//using UnityEditor.SceneManagement;
//using UnityEngine.SceneManagement;

//[InitializeOnLoad]
//public static class AlwaysPlayFromScene0
//{
//    static AlwaysPlayFromScene0()
//    {
//        EditorApplication.playModeStateChanged += OnPlayModeChanged;
//    }

//    private static void OnPlayModeChanged(PlayModeStateChange state)
//    {
//        if (state == PlayModeStateChange.ExitingEditMode)
//        {
//            string firstScenePath = SceneUtility.GetScenePathByBuildIndex(0);

//            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
//            {
//                EditorSceneManager.OpenScene(firstScenePath);
//            }
//            else
//            {
//                EditorApplication.isPlaying = false;
//            }
//        }
//    }
//}
