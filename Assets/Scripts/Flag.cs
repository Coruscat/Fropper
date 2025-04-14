using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    [SerializeField] public LayerMask player;
    [SerializeField] public List<GameObject> players;
    private List<GameObject> currentPlayers;

    void Start()
    {
        currentPlayers = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D touchPlayer = Physics2D.OverlapBox(this.transform.position, new Vector2(1.5f, 1.5f), 0f, player);
        if (Physics2D.OverlapBox(this.transform.position, new Vector2(1.5f,1.5f), 0f,player))
        {
            currentPlayers.Add(touchPlayer.gameObject);
            touchPlayer.gameObject.SetActive(false);
            players.Remove(touchPlayer.GetComponent<GameObject>());
            players[0].GetComponent<TurnLock>().turnLock = false;
        }

        if (currentPlayers.Count == players.Count)
        {
            // win condition
        }
    }
}
