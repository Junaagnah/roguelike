using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderBoard : MonoBehaviour
{
    private List<Score> scoresEasy;
    private List<Score> scoresMedium;
    private List<Score> scoresHard;
    private Cbdd bdd = new Cbdd();
    private bool MediumFilled = false;
    private bool HardFilled = false;

    private TextMeshProUGUI username;
    private TextMeshProUGUI score;

    // Start is called before the first frame update
    void Awake()
    {
        //Récupère les scores depuis la bdd
        scoresEasy = bdd.GetScores(1);
        scoresMedium = bdd.GetScores(2);
        scoresHard = bdd.GetScores(3);
    }

    private void Start()
    {
        //Remplis le tableau des scores en facile
        for (int i = 0; i < scoresEasy.Count; i++)
        {
            username = GameObject.Find("EasyPseudo" + (i + 1)).GetComponent<TextMeshProUGUI>();
            score = GameObject.Find("EasyScore" + (i + 1)).GetComponent<TextMeshProUGUI>();

            username.text = scoresEasy[i].PseudoUser;
            score.text = scoresEasy[i].ScoreUser.ToString();
        }
    }

    //Remplis le tableau des scores en moyen
    public void FillMedium()
    {
        if (!MediumFilled)
        {
            for (int i = 0; i < scoresMedium.Count; i++)
            {
                username = GameObject.Find("MediumPseudo" + (i + 1)).GetComponent<TextMeshProUGUI>();
                score = GameObject.Find("MediumScore" + (i + 1)).GetComponent<TextMeshProUGUI>();

                username.text = scoresMedium[i].PseudoUser;
                score.text = scoresMedium[i].ScoreUser.ToString();
            }

            MediumFilled = true;
        }
    }

    //Remplis le tableau des scores en difficile
    public void FillHard()
    {
        if (!HardFilled)
        {
            for (int i = 0; i < scoresHard.Count; i++)
            {
                username = GameObject.Find("HardPseudo" + (i + 1)).GetComponent<TextMeshProUGUI>();
                score = GameObject.Find("HardScore" + (i + 1)).GetComponent<TextMeshProUGUI>();

                username.text = scoresHard[i].PseudoUser;
                score.text = scoresHard[i].ScoreUser.ToString();
            }

            HardFilled = true;
        }
    }
}
