using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Frogger : MonoBehaviour
{
    [SerializeField] TextAsset level1;
    [SerializeField] TextAsset level2;
    [SerializeField] TextAsset level3;

    [SerializeField] TextAsset qData1;
    [SerializeField] TextAsset qData2;
    [SerializeField] TextAsset qData3;

    private GameObject levelContainer;

    [Header("Prefabs")]
    public GameObject log;
    public GameObject water;
    public GameObject bud;
    public GameObject lilypad;
    public GameObject croc;
    public GameObject flag;
    public GameObject p1;
    public GameObject p2;

    private float tileSize = 1.5f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1)) LoadLevel(level1, qData1, false);
        if (Input.GetKeyUp(KeyCode.Alpha2)) LoadLevel(level2, qData2, false);
        if (Input.GetKeyUp(KeyCode.Alpha3)) LoadLevel(level3, qData3, false);

        if (Input.GetKeyUp(KeyCode.Alpha4)) LoadLevel(level1, qData1, true);
        if (Input.GetKeyUp(KeyCode.Alpha5)) LoadLevel(level2, qData2, true);
        if (Input.GetKeyUp(KeyCode.Alpha6)) LoadLevel(level3, qData3, true);
    }

    void LoadLevel(TextAsset level, TextAsset Qdata, bool Q)
    {
        if (levelContainer != null)
        {
            Destroy(levelContainer);
        }

        levelContainer = new GameObject("levelContainer");
        string levelText = level.text;

        string[] lines = levelText.Trim().Split('\n');
        int height = lines.Length;

        GameObject player = null;
        GameObject player2 = null;
        GameObject flag1 = null;

        for (int i = 0; i < height; i++)
        {
            string line = lines[height - 1 - i];
            for (int j = 0; j < line.Length; j++)
            {
                char tile = line[j];
                Vector2 position = new Vector2(j * tileSize - 8, i * tileSize - 4);
                GameObject obj = null;

                switch (tile)
                {
                    case '#':
                        obj = Instantiate(log, position, Quaternion.identity, levelContainer.transform);
                        break;

                    case '-':
                        obj = Instantiate(lilypad, position, Quaternion.identity, levelContainer.transform);
                        break;

                    case 'C':
                        obj = Instantiate(croc, position, Quaternion.identity, levelContainer.transform);
                        break;

                    case 'F':
                        obj = Instantiate(lilypad, position, Quaternion.identity, levelContainer.transform);
                        flag1 = Instantiate(flag, position, Quaternion.identity, levelContainer.transform);
                        break;

                    case '1':
                        obj = Instantiate(lilypad, position, Quaternion.identity, levelContainer.transform);
                        player = Instantiate(p1, position, Quaternion.identity, levelContainer.transform);
                        break;

                    case '2':
                        obj = Instantiate(lilypad, position, Quaternion.identity, levelContainer.transform);
                        player2 = Instantiate(p2, position, Quaternion.identity, levelContainer.transform);
                        break;

                    case 'B':
                        obj = Instantiate(bud, position, Quaternion.identity, levelContainer.transform);
                        obj = Instantiate(lilypad, position, Quaternion.identity, levelContainer.transform);
                        break;

                    case 'I':
                        obj = Instantiate(bud, position, Quaternion.identity, levelContainer.transform);
                        obj = Instantiate(croc, position, Quaternion.identity, levelContainer.transform);
                        break;

                    default:
                        break;
                }
                if (obj != null) obj.transform.parent = levelContainer.transform;
            }
        }
        player.GetComponent<FrogMovement>().frogAI = player2;
        player2.GetComponent<FrogAI>().frog = player;
        player2.GetComponent<FrogAI>().Qfrog = Q;
        player2.GetComponent<FrogAI>().levelQtable = Qdata;
        flag1.GetComponent<Flag>().players.Add(player);
        flag1.GetComponent<Flag>().players.Add(player2);
    }
}
