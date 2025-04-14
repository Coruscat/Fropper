using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogMovement : MonoBehaviour
{
    [SerializeField] GameObject frog;
    [SerializeField] public GameObject frogAI;
    [SerializeField] LayerMask Goal;
    [SerializeField] LayerMask Log;
    [SerializeField] LayerMask player;
    [SerializeField] LayerMask lilypad;
    [SerializeField] LayerMask push;
    [SerializeField] LayerMask water;
    public bool canMove;
    private float moveDistance = 1.5f; // Distance to move
    private Vector2 boxSize = new Vector2(1.4f, 1.4f); // Size of the overlap box

    void Start()
    {
        canMove = true;
    }

    void Update()
    {
        Color gray = this.GetComponent<SpriteRenderer>().color;
        gray.a = 1f;
        if (!canMove && this.GetComponent<TurnLock>().turnLock)
        {
            gray.a = .5f;
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveFrog(Vector2.right);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            MoveFrog(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            MoveFrog(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            MoveFrog(Vector2.up);
        }
        this.GetComponent<SpriteRenderer>().color = gray;
    }

    private void MoveFrog(Vector2 direction)
    {
        Vector2 newPosition = (Vector2)frog.transform.position + direction * moveDistance;

        // Boundary checks
        if (newPosition.x < 10 && newPosition.x > -10 && newPosition.y < 5 && newPosition.y > -5)
        {
            Collider2D hitBox = Physics2D.OverlapBox(newPosition, boxSize, 0f, push);
            Collider2D hitLilypad = Physics2D.OverlapBox(newPosition, boxSize, 0f, lilypad);
            Collider2D playerCheck = Physics2D.OverlapBox(newPosition, boxSize, 0f, player);

            Vector2 boxTargetPos = newPosition + direction * moveDistance;
            bool isOccupied = Physics2D.OverlapBox(boxTargetPos, boxSize, 0f, push | player | Log | Goal);

            if (hitBox != null)
            {    
                if (!isOccupied)
                {
                    hitBox.transform.position = boxTargetPos;
                    frog.transform.position = newPosition;
                    SetFrogRotation(direction);
                    canMove = false;
                    frogAI.GetComponent<FrogAI>().canMove = !canMove;
                }
            }
            else if (hitLilypad != null && !playerCheck)
            {
                frog.transform.position = newPosition;
                SetFrogRotation(direction);
                canMove = false;
                frogAI.GetComponent<FrogAI>().canMove = !canMove;
            }
        }
    }

    private void SetFrogRotation(Vector2 direction)
    {
        frog.transform.rotation = Quaternion.Euler(0, 0,
            direction == Vector2.right ? -90 :
            direction == Vector2.left ? 90 :
            direction == Vector2.down ? 180 : 0);
    }


}

