using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cette classe permet de gérer les instances d'ennemis
public class Enemy : MovingObject
{
    public int mobStrength;
    public int hpMob;
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

    // Fonction appelée lors de la création d'un instance d'ennemi
    protected override void Start()
    {
        // On ajoute l'ennemi a la liste des ennemis
        GameManager.instance.AddEnemyToList(this);
        // On récupère son controlleur d'animation
        animator = GetComponent<Animator>();
        // On récupère la position initiale du joueur
        target = GameObject.FindGameObjectWithTag("Player").transform;
        // On ajuste la force des ennemis en fonction de la difficulté
        realMobStrength = mobStrength + (int)Mathf.Log(GameManager.instance.level, 2f);
        // On ajuste les points de vie des ennemis en fonction de la difficulté
        realMobHp = hpMob + ((int)Mathf.Log(GameManager.instance.level, 2f) * 2);
        if (this.gameObject.tag == "Boss")
        {
            realMobHp += GameManager.instance.level;
        }
        base.Start();
    }
    // Fonction issue de la classe MovingObject override par la classe Enemy, elle est appelée a chaque déplacement d'une instance d'ennemi
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        // On vérifie sir l'ennemi est un Boss
        if (gameObject.tag == "Boss")
        {
            // On vérifie si l'ennemi doit passer son tour à l'aide du boolean SkipMove
            if (skipMove)
            {
                // Le Boss ne peux se déplacer que tout les 4 tours
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
        // Si l'ennemi n'est pas un Boss
        else
        {
            // On vérifie si l'ennemi doit passer son tour à l'aide du boolean SkipMove
            if (skipMove)
            {
                // Un ennemi classique peux se déplacer un tour sur deux
                skipMove = false;
                return;
            }
        }

        // Si l'ennemi peux se déplacer on set skipMove a true pour empecher le déplacement au prochain tour
        skipMove = true;

        // On appele la fonction de base AttemptMove qui est définie dans la classe MovingObject
        base.AttemptMove<T>(xDir, yDir);
    }

    // Fonction permettant aux ennemis de choisir la direction dans laquelle ils doivent se déplacer 
    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        // On compare la position du Joueur et de l'enemi
        // En fonction du résultat l'ennemi se déplace dans une direction pour se rapprocher du Joueur
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            yDir = target.position.y > transform.position.y ? 1 : -1;
        else
            xDir = target.position.x > transform.position.x ? 1 : -1;

        // On appele la fonction AttemptMove de l'ennemi qui est définie plus haut
        AttemptMove<Player>(xDir, yDir);
    }

    // Fonction appelée lorsqu'un ennemi entre en collision avec un joueur
    protected override void AttackPlayer(Player player)
    {
        float damageFloat = (float)(realMobStrength * Difficulty.selected.DmgMob);
        player.LoseFood(Random.Range((int)Mathf.Ceil(damageFloat), realMobStrength));
        animator.SetTrigger("enemyAttack");
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }
    // Fonction appelée lorsqu'un ennemi entre en collision avec un mur
    protected override void BreakWall(Wall wall)
    {
        wall.DamageWall(wallDamage);
        animator.SetTrigger("enemyAttack");
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
        skipMove = false;
    }
    // L'implémentation de cette méthode est obligatoire mais n'est pas utilisée, un ennemi n'attaquera jamais un autre ennemi
    protected override void AttackEnnemy(Enemy enemy) { }

    // Fonction appelée lorsqu'un ennemi perd des points de vie
    public void DamageMob(int loss)
    {
        realMobHp -= loss;
        skipMove = false;

        if (realMobHp <= 0)
        {
            if (gameObject.tag == "Boss")
            {
                // Des actions supplémentaires sont produites si l'ennemi est un Boss
                GameManager.instance.boardScript.InstantiateExit();
                GameManager.instance.bossIsAlive = false;
                SoundManager.instance.bossMusic.Stop();
                SoundManager.instance.musicSource.Play();
            }
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
