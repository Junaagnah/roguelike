using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
    public int wallDamage = 1;
    public int playerStrength;
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

        foodText.text = "Food: " + food;
        playerLvlText.text = "LVL " + lvl;
        playerMoneyText.text = "Coins: " + money;
        playerXpText.text = "XP: " + xp + "/100";
        playerStrengthText.text = "Strength: " + playerStrength;


        base.Start();
    }

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
    void Update()
    {
        if (!GameManager.instance.playersTurn) return;

        Debug.Log("avant clean de la liste");
        GameManager.instance.mobMovePos.Clear();
        Debug.Log("Après clean de la liste");

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;

        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical);
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        turns++;

        foodText.text = "Food: " + food;
        playerStrengthText.text = "Strength: " + playerStrength;

        base.AttemptMove<T>(xDir, yDir);

        if (playerCanMove)
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }
        //RaycastHit2D hit;
        //if (Move(xDir, yDir, out hit))
        //{
        //    SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        //}

        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.tag == "Food")
        {
            int foodGain = Random.Range(pointsPerFoodMin, pointsPerFoodMax);
            food += foodGain;
            foodText.text = "+" + foodGain + " Food: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
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
            playerStrengthText.text = strengthText + " Strength: " + playerStrength;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound1);
            other.gameObject.SetActive(false);
        }
    }

    protected override void BreakWall(Wall wall)
    {
        wall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
    }

    protected override void AttackEnnemy(Enemy enemy)
    {
        float damageFloat = (float) (playerStrength * Difficulty.selected.DmgPlayer);
        enemy.DamageMob(Random.Range((int)Mathf.Ceil(damageFloat), playerStrength));
        if (enemy.realMobHp <= 0)
        {
            XpGain(enemy.xpGiven);
            MoneyGain(enemy.moneyGiven);
            monsterKilled++;
        }
        animator.SetTrigger("playerChop");
        GetComponent<AudioSource>().PlayOneShot(swooshSound);
        foodText.text = "Food: " + food;
        playerMoneyText.text = "Coins: " + money;
    }

    protected override void AttackPlayer(Player player) { }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseFood (int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        GetComponent<AudioSource>().PlayOneShot(getHit);
        foodText.text = "-" + loss + " Food: " + food;
        CheckIfGameOver();
    }

    private void XpGain (int xpGained)
    {
        xp += xpGained;

        if (xp >= 100)
        {
            lvl++;
            playerStrength++;
            food += 25;
            xp -= 100;
            playerLvlText.text = "LVL " + lvl;
            playerXpText.text = "XP: " + xp + "/100";
            playerStrengthText.text = "Strength: " + playerStrength;
        }
        playerXpText.text = "XP: " + xp + "/100";
    }

    private void MoneyGain (int moneyGained)
    {
        float moneyFloat = (float) (moneyGained * Difficulty.selected.CoefMoney);
        money += (int)Mathf.Ceil(moneyFloat);
    }

    //private void CheckIfGameOver()
    //{
    //    if (food <= 0)
    //    {
    //        SoundManager.instance.PlaySingle(gameOverSound);
    //        SoundManager.instance.musicSource.Stop();
    //        SoundManager.instance.gameOverMusic.Play();
    //        GameManager.instance.GameOver();
    //    }
    //}

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            SoundManager.instance.musicSource.Stop();
            StartCoroutine(DCD());
        }
    }

    IEnumerator DCD()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        SoundManager.instance.PlaySingle(gameOverSound);
        yield return new WaitForSecondsRealtime(0.1f);
        SoundManager.instance.gameOverMusic.Play();
        GameManager.instance.GameOver();
    }
}
