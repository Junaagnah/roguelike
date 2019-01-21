using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private Cbdd bdd = new Cbdd();

    public void Awake()
    {
        if (File.Exists("user"))
        {
            Score.idUserTemp = int.Parse(File.ReadAllText("user"));

            Difficulty.difficulties = bdd.GetDifficulties();

            Difficulty.selected = Difficulty.difficulties[0];
        }
        else
        {
            Application.Quit();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
