using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

// Cette classe permet de gérer l'entièretée de l'application
public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    private float turnDelay = 0.23f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 200;
    public int playerLvl = 1;
    public int playerXp = 0;
    public int playerMoney;
    public int playerMonsterKilled;
    public int playerBossKilled = 0;
    public int playerStrength = 300;
    public int playerTurns = 0;
    public int level = 1;
    [HideInInspector] public bool playersTurn = true;
    public bool enemyIsMoving;
    [HideInInspector] public int bossTurn = 0;
    public Vector2 playerPosition;
    public bool bossIsAlive;

    private Text levelText;
    private Text gameOverText;
    private Text levelTextUI;
    private GameObject levelImage;
    [HideInInspector] public List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;
    private bool firstRun = true;
    private bool gameOver = false;
    private float waitingTime;
    private float timeToWait;
    private int seconds;
    private int milliseconds;

    public List<Vector2> mobMovePos = new List<Vector2>();

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

    //Incrémente le niveau et crée une nouvelle carte
    private void OnLevelWasLoaded(int index)
    {
        if (firstRun)
        {
            firstRun = false;
            return;
        }

        level++;
        InitGame();

        //Si le niveau est un multiple de 10, on lance la musique du boss
        if (level % 10 == 0)
        {
            SoundManager.instance.musicSource.Stop();
            SoundManager.instance.bossMusic.Play();
            return;
        }
    }

    //Initialise le jeu avec les valeurs par défaut et crée la map
    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelTextUI = GameObject.Find("Etage").GetComponent<Text>();
        gameOverText = GameObject.Find("GameOverText").GetComponent<Text>();
        levelText.text = "Étage " + level;
        levelTextUI.text = levelText.text;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);
        bossTurn = 0;
        bossIsAlive = true;

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    //Cache l'image affichant le niveau
    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    //Affiche l'écran de Game Over
    public void GameOver()
    {
        gameOver = true;
        levelText.text = "Après " + level + " étages, vous êtes mort.";
        gameOverText.text = "Appuyez sur R pour recommencer. Appuyez sur Échap pour revenir au menu.";
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
        //Détruit tous les éléments et réinitialise la scène quand le joueur appuie sur R pour recommencer
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            foreach (GameObject Object in GameObject.FindObjectsOfType<GameObject>())
            {
                Destroy(Object);
            }
        }

        //Détruit tous les éléments et retourne au menu quand le joueur appuie sur Echap
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

            foreach (GameObject Object in GameObject.FindObjectsOfType<GameObject>())
            {
                Destroy(Object);
            }
        }

        //Empêche les ennemis de bouger
        if (playersTurn || enemiesMoving || doingSetup)
        {
            if (playersTurn)
            {
                //Permet au joueur de passer son tour
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    playersTurn = false;
                }
            }
            return;
        }    

        //Si ce n'est pas le tour du joueur, on appelle la coroutine gérant les mouvements des ennemis
        StartCoroutine(MoveEnemies());
    }

    //Ajout un ennemi à la liste des ennemis
    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        bossTurn++;
        enemiesMoving = true;
        DateTime start = DateTime.Now;
        yield return new WaitForSeconds(0.1f);

        if (level % 10 == 0)
        {
            if (bossTurn != 0 && bossTurn % 20 == 0 && bossIsAlive)
            {
                this.boardScript.BossInvokeMob();
            }
            else if (bossTurn != 0 && bossTurn % 15 == 0 && bossIsAlive)
            {
                this.boardScript.FoodInvoke();
            }
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
                yield return new WaitForSeconds(0.01f);
            }
        }
        DateTime end = DateTime.Now;
        TimeSpan timeDiff = end - start;
        seconds = timeDiff.Seconds;
        milliseconds = timeDiff.Milliseconds;
        waitingTime = (float)seconds + ((float)milliseconds / 1000);
        timeToWait = turnDelay - waitingTime;
        yield return new WaitForSeconds(timeToWait);
        Debug.Log(timeToWait);
        enemiesMoving = false;
        playersTurn = true;
    }
}