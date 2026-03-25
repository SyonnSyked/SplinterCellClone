using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    public void Resume()
    {
        GameManager.instance.StateUnpause();
    }


    public void Restart()
    {
        GameManager.instance.RespawnPlayer();
        GameManager.instance.StateUnpause();
    }

    public void Options()
    {
        GameManager.instance.OptionsScreen();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#else
        Application.Quit();
#endif

    }
}
