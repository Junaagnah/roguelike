using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 200;
    public int playerLvl = 1;
    public int playerXp = 0;
    public int playerMoney;
    public int playerMonsterKilled;
    public int playerBossKilled = 0;
    public int playerStrength = 3;
    public int playerTurns = 0;
    public int level = 1;
    [HideInInspector] public bool playersTurn = true;

    private Text levelText;
    private Text gameOverText;
    private Text levelTextUI;
    private GameObject levelImage;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;
    private bool firstRun = true;
    private bool gameOver = false;
    private Score score = new Score(Score.idUserTemp);
    private Cbdd bdd = new Cbdd();

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    private void OnLevelWasLoaded(int index)
    {
        if (firstRun)
        {
            firstRun = false;
            return;
        }

        level++;
        InitGame();
    }

    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelTextUI = GameObject.Find("Etage").GetComponent<Text>();
        gameOverText = GameObject.Find("GameOverText").GetComponent<Text>();
        levelText.text = "Etage " + level;
        levelTextUI.text = levelText.text;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        gameOver = true;
        levelText.text = "After " + level + " levels, you died.";
        gameOverText.text = "Press R to restart. Press Escape to quit.";
        levelImage.SetActive(true);
        GameObject.FindGameObjectWithTag("Player").SetActive(false);

        //Si le joueur n'est pas hors ligne, on enregistre son score
        if (Difficulty.selected.Nom != "Offline")
        {
            score.SaveGame(level, playerStrength, playerMoney, playerTurns, playerMonsterKilled, playerBossKilled);
            bdd.SaveGame(score, Difficulty.selected);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (gameOver)
            {
                gameOver = false;
            } else
            {
                GameObject.FindGameObjectWithTag("Player").SetActive(false);
            }
            score = new Score(Score.idUserTemp);
            gameOverText.text = "";
            level = 0;
            playerFoodPoints = 200;
            playerStrength = 3;
            playerLvl = 1;
            playerMoney = 0;
            playerXp = 0;
            playerTurns = 0;
            playerMonsterKilled = 0;
            playerBossKilled = 0;
            SoundManager.instance.gameOverMusic.Stop();
            SoundManager.instance.musicSource.Play();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (playersTurn || enemiesMoving || doingSetup)
        {
            if (playersTurn)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    playersTurn = false;
                }
            }
            return;
        }    

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].realMobHp <= 0)
            {
                enemies.RemoveAt(i);
            }
            else
            {
                enemies[i].MoveEnemy();
                yield return new WaitForSeconds(enemies[i].moveTime);
            }
        }

        playersTurn = true;
        enemiesMoving = false;
    }
}
