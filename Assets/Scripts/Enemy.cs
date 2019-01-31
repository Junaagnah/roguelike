using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int mobStrength;
    public int hpMob = 10;
    [HideInInspector] public int realMobHp;
    public int wallDamage = 2;
    public int xpGiven;
    public int moneyGiven;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    private Animator animator;
    private Transform target;
    private bool skipMove;
    private int realMobStrength;

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        realMobStrength = mobStrength + (int)Mathf.Log(GameManager.instance.level, 2f);
        realMobHp = hpMob + ((int)Mathf.Log(GameManager.instance.level, 2f) * 2);
        base.Start();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (gameObject.tag == "Boss")
        {
            if (skipMove)
            {
                if (GameManager.instance.bossTurn % 4 == 0)
                {
                    skipMove = false;
                    return;
                }
                else
                {
                    return;
                }
            }
        }
        else
        {
            if(skipMove)
            {
                skipMove = false;
                return;
            }
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
        float damageFloat = (float)(realMobStrength * Difficulty.selected.DmgMob);
        player.LoseFood(Random.Range((int)Mathf.Ceil(damageFloat), realMobStrength));
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
        realMobHp -= loss;
        skipMove = false;

        if (realMobHp <= 0)
        {
            if (gameObject.tag == "Boss")
            {
                GameManager.instance.boardScript.InstantiateExit();
                GameManager.instance.bossIsAlive = false;
            }
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
