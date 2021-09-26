using UnityEngine;
using UnityEngine.SceneManagement;

public class DrawSceneManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            var scene = SceneManager.GetActiveScene();
            if (scene.buildIndex != 1)
            {
                SceneManager.LoadScene(1);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            var scene = SceneManager.GetActiveScene();
            if (scene.buildIndex != 2)
            {
                SceneManager.LoadScene(2);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            var scene = SceneManager.GetActiveScene();
            if (scene.buildIndex != 3)
            {
                SceneManager.LoadScene(3);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha0))
        {
            var scene = SceneManager.GetActiveScene();
            if (scene.buildIndex != 0)
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}
