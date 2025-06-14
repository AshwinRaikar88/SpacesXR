using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Quits the application. Will not work in the editor.
    /// </summary>
    public void QuitApplication()
    {
        Debug.Log("Quitting application...");

#if UNITY_EDITOR
        // Stop play mode if running in the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quit the built application
        Application.Quit();
#endif
    }
}
