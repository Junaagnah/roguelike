using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;
    public int hpMob = 10;
    public int wallDamage = 2;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;
    public AudioClip chopSound1;
    public AudioClip chopSound2;

    private Animator animator;
    private Transform target;
    private bool skipMove;

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

        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;
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
        player.LoseFood(playerDamage);
        animator.SetTrigger("enemyAttack");
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }
    protected override void BreakWall(Wall wall)
    {
        wall.DamageWall(wallDamage);
        animator.SetTrigger("ennemyAttack");
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }

    protected override void AttackEnnemy(Enemy enemy) { }

    public void DamageMob(int loss)
    {
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
        hpMob -= loss;

        if (hpMob <= 0)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
