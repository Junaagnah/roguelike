using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    private List<Score> scoresEasy;
    private List<Score> scoresMedium;
    private List<Score> scoresHard;
    private Cbdd bdd = new Cbdd();

    // Start is called before the first frame update
    void Awake()
    {
        scoresEasy = bdd.GetScores(1);
        scoresMedium = bdd.GetScores(2);
        scoresMedium = bdd.GetScores(3);
    }
}
