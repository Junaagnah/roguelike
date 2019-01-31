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
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();

        inverseMovetime = 8.5f;
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        foreach (Vector2 movePos in GameManager.instance.mobMovePos)
        {
            if (end == movePos)
            {
                mobCanMove = false;
                break;
            }
        }

        GameManager.instance.mobMovePos.Add(end); 

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);

        boxCollider.enabled = true;

        if (hit.transform == null && mobCanMove)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        else
        {
            this.mobCanMove = true;
            return false;
        }
    }

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

    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
        {
            playerCanMove = true;
            return;
        }
        playerCanMove = false;

        Enemy enemy = hit.transform.GetComponent<Enemy>();
        Wall wall = hit.transform.GetComponent<Wall>();
        Player player = hit.transform.GetComponent<Player>();

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
    }

    protected abstract void BreakWall(Wall wall);
    protected abstract void AttackEnnemy(Enemy enemy);
    protected abstract void AttackPlayer(Player player);
}
