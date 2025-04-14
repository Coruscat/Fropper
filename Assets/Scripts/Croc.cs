using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Croc : MonoBehaviour
{
    private bool hasVisited;
    private bool hasExited;
    public LayerMask player;
    // Update is called once per frame
    void Update()
    {
        if (Physics2D.OverlapBox(this.transform.position,new Vector2(1.4f,1.4f),0f,player))
        {
            hasVisited = true;
        }

        if (!Physics2D.OverlapBox(this.transform.position, new Vector2(1.4f, 1.4f), 0f, player) && hasVisited)
        {
            this.gameObject.layer = default;
            Color gray = this.GetComponent<SpriteRenderer>().color;
            gray.a = .5f;
            this.GetComponent<SpriteRenderer>().color = gray;
        }
    }
}
