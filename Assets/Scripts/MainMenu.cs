using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private Cbdd bdd = new Cbdd();
    private string userFile = "user";

    public void Awake()
    {
        if (File.Exists(userFile))
        {
            if (File.ReadAllText(userFile) == "offline")
            {
                //@TODO: créer une difficulté par défaut & empêcher le joueur de sauvegarder son score
            }
            else
            {
                Score.idUserTemp = int.Parse(File.ReadAllText(userFile));

                Difficulty.difficulties = bdd.GetDifficulties();

                Difficulty.selected = Difficulty.difficulties[0];
            }
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
