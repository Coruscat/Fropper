using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using static FrogQLearner;

public class FrogAI : MonoBehaviour
{
    [SerializeField] public GameObject frog;
    [SerializeField] LayerMask Goal;
    [SerializeField] LayerMask Log;
    [SerializeField] LayerMask player;
    [SerializeField] LayerMask lilypad;
    [SerializeField] LayerMask push;
    [SerializeField] LayerMask water;

    public TextAsset levelQtable;
    private Dictionary<string, float[]> QTable;

    public bool canMove;
    public bool isMoving;
    public bool Qfrog = false;

    private float moveDistance = 1.5f;
    private Vector2 boxSize = new Vector2(1.4f, 1.4f);
    private Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    void Start()
    {
        canMove = false;
        isMoving = false;
        QTable = new Dictionary<string, float[]>();

        if (Qfrog)
        {
            QWrapper wrapper = JsonUtility.FromJson<QWrapper>(levelQtable.text);
            foreach (var entry in wrapper.items)
            {
                QTable[entry.state] = entry.qValues;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Color gray = this.GetComponent<SpriteRenderer>().color;
        gray.a = .5f;

        if (canMove && !isMoving)
        {

            StartCoroutine(DelayedAIMove());
        }
        else if (canMove)
        {
            gray.a = 1f;
        }
        this.GetComponent<SpriteRenderer>().color = gray;

        if (!this.GetComponent<TurnLock>().turnLock)
        {
            StartCoroutine (DelayedAIMove());
        }
    }

    private Vector2 RandomDir()
    {
        return directions[Random.Range(0, 4)];
    }


    private void Move(Vector2 direction)
    {
        Vector2 newPosition = (Vector2)this.transform.position + direction * moveDistance;

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
                    this.transform.position = newPosition;
                    SetFrogRotation(direction);
                    canMove = false;
                }
            }
            else if (hitLilypad != null && !playerCheck)
            {
                this.transform.position = newPosition;
                SetFrogRotation(direction);
                canMove = false;
            }
        }
    }

    IEnumerator DelayedAIMove()
    {
        isMoving = true;
        yield return new WaitForSeconds(.25f);
        if (Qfrog)
        {
            string state = GetState(); 
            if (QTable.TryGetValue(state, out float[] qValues))
            {
                int action = MaxIndex(qValues);
                Vector2 direction = directions[action];
                Move(direction);
            }
            else
            {
                Debug.LogWarning("State not found in Q-table: " + state + ". Choosing random.");
                Move(RandomDir());
            }
        }
        else
        {
            Move(RandomDir());
        }
        frog.GetComponent<FrogMovement>().canMove = !canMove;
        isMoving = false;
    }

    private string GetState()
    {
        //convert from tiles to logic space
        Vector2 AIfrogPos = new Vector2((transform.position.x + 8f) / 1.5f,(transform.position.y + 4f) / 1.5f);
        Vector2 frogPos = new Vector2((frog.transform.position.x + 8f) / 1.5f, (frog.transform.position.y + 4f) / 1.5f);
        Vector2Int aiPos = Vector2Int.RoundToInt(AIfrogPos);
        Vector2Int playerPos = Vector2Int.RoundToInt(frogPos); 
        return $"{aiPos.x},{aiPos.y},{playerPos.x},{playerPos.y}";
    }

    private int MaxIndex(float[] values)
    {
        int best = 0;
        for (int i = 1; i < values.Length; i++)
            if (values[i] > values[best]) best = i;
        return best;
    }


    private void SetFrogRotation(Vector2 direction)
    {
        this.transform.rotation = Quaternion.Euler(0, 0,
            direction == Vector2.right ? -90 :
            direction == Vector2.left ? 90 :
            direction == Vector2.down ? 180 : 0);
    }
}
