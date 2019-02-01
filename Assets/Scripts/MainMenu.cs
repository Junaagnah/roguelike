using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
                //Si le joueur est hors-ligne, on assigne automatiquement la difficulté facile et on la nomme Offline pour plus tard
                Difficulty.selected = new Difficulty(0, "Offline", 11, (decimal)0.9, (decimal)0.3, 0, 8, 12, 3, 4, 1);

                //On rend les boutons scores et options inactifs car ils sont dépendants de la base de données
                GameObject.Find("ScoresButton").SetActive(false);
                GameObject.Find("OptionsButton").SetActive(false);
            }
            else
            {
                //Sinon, on récupère l'id de l'user dans le fichier et les difficultés depuis la bdd
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

    public void CheckOffline()
    {
        if (Difficulty.selected.Nom == "Offline")
        {
            PlayGame();
        }
    }
}
