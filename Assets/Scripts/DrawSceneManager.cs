using UnityEngine;
using UnityEngine.SceneManagement;

public class DrawSceneManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.H))
        {
            var scene = SceneManager.GetActiveScene();
            if (scene.buildIndex != 0)
            {
                SceneManager.LoadScene(0);
            }
            else if (scene.buildIndex != 1)
            {
                SceneManager.LoadScene(1);
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
