using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bud : MonoBehaviour
{
    public LayerMask lilypad;
    public LayerMask water;
    public GameObject lilyPrefab;

    private Vector2 checkSize = new Vector2(1.4f, 1.4f); // Area to check for overlaps

    void Update()
    {
        bool isTouchingLily = Physics2D.OverlapBox(transform.position, checkSize, 0f, lilypad);

        if (!isTouchingLily)
        {
            GrowIntoLily();
        }
    }

    void GrowIntoLily()
    {
        Instantiate(lilyPrefab, transform.position + new Vector3(0,0,.3f), Quaternion.identity,this.transform.parent);
        
        Destroy(this.gameObject);
    }
}
