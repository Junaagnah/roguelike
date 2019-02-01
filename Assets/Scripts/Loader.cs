using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script permettant d'instancier le jeu
public class Loader : MonoBehaviour
{
    public GameObject gameManager;

    // Start is called before the first frame update
    void Awake()
    {
        //Instantie le Game Manager
        if (GameManager.instance == null)
        {
            Instantiate(gameManager);
        }
    }
}
