using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


// Cette classe permet de gérer l'instance du Joueur
public class Player : MovingObject
{
    public int wallDamage = 1;
    private int playerStrength;
    public int pointsPerFoodMin = 10;
    public int pointsPerFoodMax = 20;
    public int strengthPotionValue;
    public float restartLevelDelay = 1;
    public Text foodText;
    public Text playerLvlText;
    public Text playerMoneyText;
    public Text playerXpText;
    public Text playerStrengthText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip getHit;
    public AudioClip chopSound1;
    public AudioClip chopSound2;
    public AudioClip swooshSound;
    public AudioClip gameOverSound;


    private Animator animator;
    private int food;
    private int lvl;
    private int xp;
    private int monsterKilled;
    private int money;
    private int turns;
    private int bossKilled;

    // Start is called before the first frame update
    // Rècupère le controlleur d'naimations de l'instance du joueur et set les variables du joueur via les données stockées dans le Gamemanager
    // Le Gamemanager reste intact entre le changement de niveau ce qui permet de récupérer les différentes variables concernant le Joueur
    protected override void Start()
    {
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints;
        lvl = GameManager.instance.playerLvl;
        xp = GameManager.instance.playerXp;
        monsterKilled = GameManager.instance.playerMonsterKilled;
        money = GameManager.instance.playerMoney;
        playerStrength = GameManager.instance.playerStrength;
        bossKilled = GameManager.instance.playerBossKilled;
        turns = GameManager.instance.playerTurns;

        foodText.text = "PV: " + food;
        playerLvlText.text = "NIVEAU " + lvl;
        playerMoneyText.text = "OR: " + money;
        playerXpText.text = "EXP: " + xp + "/100";
        playerStrengthText.text = "FORCE: " + playerStrength;


        base.Start();
    }
    // Les Variables concernant le joueur sont retournées et stockées dans le Gamemanager
    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
        GameManager.instance.playerLvl = lvl;
        GameManager.instance.playerXp = xp;
        GameManager.instance.playerMonsterKilled = monsterKilled;
        GameManager.instance.playerMoney = money;
        GameManager.instance.playerStrength = playerStrength;
        GameManager.instance.playerTurns = turns;
        GameManager.instance.playerBossKilled = bossKilled;
    }

    // Update is called once per frame
    // On vérifie à chaque frame si c'est le tour du joueur
    // Si c'est le cas on récupère la direction dans laquelle il souhaite se déplacer pour la passer a la fonction AttemptMove qui est override par la classe Player
    void Update()
    {
        if (!GameManager.instance.playersTurn) return;

        GameManager.instance.mobMovePos.Clear();

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        // On empêche le déplacement Horizontal
        if (horizontal != 0)
            vertical = 0;

        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical);
    }
    // Fonction issue de la classe MovingObject override par la classe Player, elle est appelée a chaque déplacement du Joueur
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        // On incrémente le nombre de tours du Joueur
        turns++;
        // On refresh l'affichage des PV et de la force
        foodText.text = "PV: " + food;
        playerStrengthText.text = "FORCE: " + playerStrength;

        // On appele la fonction de base AttemptMove qui est définie dans la classe MovingObject
        base.AttemptMove<T>(xDir, yDir);

        // On vérifie que le joueur peux se déplacer grâce à un boolean
        if (playerCanMove)
        {
            // Si le déplacement est possible on joue un son de déplacement
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        // Le tour du joueur est finit
        GameManager.instance.playersTurn = false;
    }

    // On traite les différents cas dans lesquels le Joueur entre en collision avec des objets ou la sortie
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Quand le joueur touche la sortie
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        // Quand le joueur récupère du Pain
        else if (other.tag == "Food")
        {
            int foodGain = Random.Range(pointsPerFoodMin, pointsPerFoodMax);
            food += foodGain;
            foodText.text = "+" + foodGain + " PV: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        // Quand le joueur récupère une potion
        else if (other.tag == "Soda")
        {
            strengthPotionValue = 0;
            while (strengthPotionValue == 0)
            {
                strengthPotionValue = Random.Range(-1, 2);
            }

            if (strengthPotionValue < 0 && playerStrength == 1)
            {
                strengthPotionValue = -1;
            }
            else
            {
                playerStrength += strengthPotionValue;
            }

            string strengthText = strengthPotionValue.ToString("+#;-#;0");
            playerStrengthText.text = strengthText + " FORCE: " + playerStrength;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound1);
            other.gameObject.SetActive(false);
        }
    }

    // Cette fonction est appelée quand le joueur entre en collision avec un mur qui est destructible
    // Une animation et un son se produisent, on appelle la fonction qui permet de faire diminuer les points de vies du mur
    protected override void BreakWall(Wall wall)
    {
        wall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
    }
    // Cette fonction est appelée quand le joueur entre en collision avec un ennemi
    protected override void AttackEnnemy(Enemy enemy)
    {
        // La Dégâts produits par le joueur sont randomizés en fonction de la difficulté
        float damageFloat = (float) (playerStrength * Difficulty.selected.DmgPlayer);
        enemy.DamageMob(Random.Range((int)Mathf.Ceil(damageFloat), playerStrength));
        if (enemy.realMobHp <= 0)
        {
            // Si l'ennemi n'a plus de points de vies il meurt et le joueur gagne de l'éxpèrience
            XpGain(enemy.xpGiven);
            MoneyGain(enemy.moneyGiven);
            if (enemy.tag == "Boss")
            {
                bossKilled++;
            }
            else
            {
                monsterKilled++;
            }
        }
        // On active l'annimation et les sons correspondant a l'attaque du joueur
        animator.SetTrigger("playerChop");
        GetComponent<AudioSource>().PlayOneShot(swooshSound);
        // On met à jour l'affichage de l'Or dans le cas ou le Joueur aurait tué l'ennemi
        playerMoneyText.text = "OR: " + money;
    }
    // L'implémentation de cette méthode est obligatoire mais n'est pas utilisée, un joueur n'attaquera jamais un joueur
    protected override void AttackPlayer(Player player) { }
    // Fonction appelée lorsque le joueur touche la sortie, elle recharge la scène qui sera alors adaptée par le Gamemanager
    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    // Fonction appelée lorsque le joueur perd des points de vies
    public void LoseFood (int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        GetComponent<AudioSource>().PlayOneShot(getHit);
        foodText.text = "-" + loss + " PV: " + food;
        CheckIfGameOver();
    }
    // Fonction appelée lorsque le joueur gagne de l'expérience
    private void XpGain (int xpGained)
    {
        xp += xpGained;
        // Lorsque le joueur dépasse les 100 d'xp il gagne un niveau et donc de la vie, de la force
        if (xp >= 100)
        {
            lvl++;
            playerStrength++;
            food += 25;
            xp -= 100;
            playerLvlText.text = "NIVEAU " + lvl;
            playerXpText.text = "EXP: " + xp + "/100";
            playerStrengthText.text = "FORCE: " + playerStrength;
        }
        playerXpText.text = "EXP: " + xp + "/100";
    }
    // Fonction appelée quand le joueur gagne de l'Or
    private void MoneyGain (int moneyGained)
    {
        float moneyFloat = (float) (moneyGained * Difficulty.selected.CoefMoney);
        money += (int)Mathf.Ceil(moneyFloat);
    }
    // Fonction permettant de vérifier si le joueur à encore des points de vies
    private void CheckIfGameOver()
    {
        // Si le joueur n'a plus de points de vies il a perdu
        if (food <= 0)
        {
            SoundManager.instance.musicSource.Stop();
            SoundManager.instance.bossMusic.Stop();
            StartCoroutine(DCD());
        }
    }
    // Fonction permettant de gérer les différentes transitions Audio et Vidéo quand le joueur a perdu
    IEnumerator DCD()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        SoundManager.instance.PlaySingle(gameOverSound);
        yield return new WaitForSecondsRealtime(0.1f);
        SoundManager.instance.gameOverMusic.Play();
        GameManager.instance.GameOver();
    }
}
