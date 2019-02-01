using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    protected float moveTime = 0.1f;
    public LayerMask blockingLayer;
    public bool playerCanMove;

    public bool mobCanMove = true;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMovetime;

    // Start is called before the first frame update
    // Cette fonction est appelée par le Joueur et les ennemis, elle permet de récupérer les composants 'BoxCollider2D' et 'Rigidbody2D'
    // Ces composants sont paramétrés sur les prefabs Player et Enemy via Unity
    // On set aussi la variable inverseMovetime qui va nous permettre de réaliser le calcul pour rendre l'animation de déplacement 'fluide'
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();

        inverseMovetime = 8.5f;
    }

    // Cette fonction est appelée par les instances des ennemis et l'instance du Joueur
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        // On récupère la position de l'instance qui a appelé la fonction
        Vector2 start = transform.position;
        // On recupère les coordonnées de la case sur laquelle l'instance essaye de se déplacer
        Vector2 end = start + new Vector2(xDir, yDir);

        // Cette boucle est utile lors du tours des monstres
        // Elle permet de récupérer directement les position sur lesquelles les monstres souhaitent se déplacer dans une liste publique
        foreach (Vector2 movePos in GameManager.instance.mobMovePos)
        {
            if (end == movePos)
            {
                // Si un monstre souhaite se déplacer sur une case déjà selectionné par un autre monstre on set le boolean mobCanMove a false
                mobCanMove = false;
                break;
            }
        }

        // On ajoute la position selectionnée par le joueur
        GameManager.instance.mobMovePos.Add(end); 


        // On teste si une collision va se produire avec un autre monstre ou le joueur lors du déplacement
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        // Si il n'y a pas de collision et que la variable mobcanmove est à true
        if (hit.transform == null && mobCanMove)
        {
            // On appele la fonction permettant le déplacement 'fluide' et la fonction renvoie true
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        // sinon si il y a une collision ou si une collision pourrait se produire lors du déplacement ( on le sait grace a la variable mobCanMove )
        else
        {
            // On reset la variable mobCanMove de l'instance a true et la fonction renvoie false
            this.mobCanMove = true;
            return false;
        }
    }

    // Cette fonction permet aux entitées de réaliser un déplacement 'fluide'
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMovetime * Time.fixedDeltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    // Cette fonction est appelée par les classes Player et Enemy, elle utilise la fonction Move située plus haut
    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        // Si le retour de Move est false c'est que l'entitée est entrée en collision avec une autre entitée
        bool canMove = Move(xDir, yDir, out hit);

        // Si il n'y a pas de collision on set la variable playerCanMove a true pour l'utiliser dans la classe Player et jouer le bruit de déplacement
        if (canMove)
        {
            playerCanMove = true;
            // Vu qu'il n'y a pas de collision on peux sortir de la fonction
            return;
        }

        // On essaye de cast le component avec lequel on est entré en collision via la variable hit
        Enemy enemy = hit.transform.GetComponent<Enemy>();
        Wall wall = hit.transform.GetComponent<Wall>();
        Player player = hit.transform.GetComponent<Player>();

        // selon le résultat du cast on appelle les fonctions correspondantes qui sont définies dans les classes Player et Enemy
        if (!canMove && enemy != null)
        {
            AttackEnnemy(enemy);
        }
        else if (!canMove && wall != null)
        {
            BreakWall(wall);
        }
        else if (!canMove && player != null)
        {
            AttackPlayer(player);
        }

        // On reset la variable playerCanMove pour le prochain tour
        playerCanMove = false;
    }

    // Les fonctions abstraites à implémentés dans les classes héritant de MovingObect
    protected abstract void BreakWall(Wall wall);
    protected abstract void AttackEnnemy(Enemy enemy);
    protected abstract void AttackPlayer(Player player);
}
