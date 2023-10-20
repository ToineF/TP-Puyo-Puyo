using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsManager : MonoBehaviour
{
    public void RestartLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index);
    }

    public void NextLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        if (index + 1 < 5)
            SceneManager.LoadScene(index + 1);
        else
            Application.Quit();
    }
}
