using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public float mobDamage;
    public int hpMob = 10;
    public int wallDamage = 2;
    public int xpGiven;
    public int moneyGiven;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    private Animator animator;
    private Transform target;
    private bool skipMove;
    private float randomDamage;
    private float coef;
    private int playerDamage;

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {

        if (skipMove)
        {
            skipMove = false;
            return;
        }

        skipMove = true;

        base.AttemptMove<T>(xDir, yDir);
    }

    public void MoveEnemy()
    {
            int xDir = 0;
            int yDir = 0;

            if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
                yDir = target.position.y > transform.position.y ? 1 : -1;
            else
                xDir = target.position.x > transform.position.x ? 1 : -1;

            AttemptMove<Player>(xDir, yDir);
    }

    protected override void AttackPlayer(Player player)
    {
        coef = 0.50f;
        randomDamage = Random.Range(mobDamage * coef, mobDamage);
        randomDamage = Mathf.Ceil(randomDamage);
        playerDamage = (int) randomDamage;
        player.LoseFood(playerDamage);
        animator.SetTrigger("enemyAttack");
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }
    protected override void BreakWall(Wall wall)
    {
        wall.DamageWall(wallDamage);
        animator.SetTrigger("enemyAttack");
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
        skipMove = false;
    }

    protected override void AttackEnnemy(Enemy enemy) { }

    public void DamageMob(int loss)
    {
        hpMob -= loss;
        skipMove = false;

        if (hpMob <= 0)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
