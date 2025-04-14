using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FrogQLearner : MonoBehaviour
{
    [SerializeField] private TextAsset levelFile;
    [SerializeField] private int episodes = 1000;
    [SerializeField] private float learningRate = 0.1f;
    [SerializeField] private float discountFactor = 0.9f;
    [SerializeField] private float epsilon = 0.2f;

    private Dictionary<string, float[]> qTable = new();
    private Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    private void Start()
    {
        StartTraining();
    }

    public void StartTraining()
    {
        char[,] grid = SimulateLevel.Parse(levelFile.text);

        for (int ep = 0; ep < episodes; ep++)
        {
            SimulateLevel level = new(grid);
            for (int step = 0; step < 200; step++)
            {
                if (step % 2 == 0 || step == 0)
                {
                    level.MovePlayerTowardGoalWithEpsilon(1f);
                }
                else
                {
                    string state = level.GetState();
                    int action = ChooseAction(state);
                    Vector2Int dir = directions[action];

                    bool moved = level.Step(dir, out float reward, out bool done);
                    string nextState = level.GetState();

                    float[] qValues = GetQValues(state);
                    float[] nextQ = GetQValues(nextState);

                    qValues[action] = qValues[action] + learningRate * (reward + discountFactor * Max(nextQ) - qValues[action]);

                    if (done) break;
                }
            }
        }
        string levelName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(levelFile.GetInstanceID()));
        SaveQTable("Assets/QData/qtable" + levelName + ".json");
        Debug.Log("Training complete.");
    }

    private int ChooseAction(string state)
    {
        float[] Q = GetQValues(state);
        if (Random.value < epsilon)
            return Random.Range(0, directions.Length);
        else
            return MaxIndex(Q);
    }

    private float[] GetQValues(string state)
    {
        if (!qTable.ContainsKey(state))
            qTable[state] = new float[directions.Length];
        return qTable[state];
    }

    private float Max(float[] arr)
    {
        float max = arr[0];
        foreach (float val in arr)
            if (val > max) max = val;
        return max;
    }

    private int MaxIndex(float[] arr)
    {
        int best = 0;
        for (int i = 1; i < arr.Length; i++)
            if (arr[i] > arr[best]) best = i;
        return best;
    }

    private void SaveQTable(string path)
    {
        var jsonData = JsonUtility.ToJson(new QWrapper(qTable));
        File.WriteAllText(path, jsonData);
    }

    [System.Serializable]
    public class QEntry
    {
        public string state;
        public float[] qValues;
    }

    [System.Serializable]
    public class QWrapper
    {
        public List<QEntry> items = new();

        public QWrapper(Dictionary<string, float[]> qTable)
        {
            foreach (var kv in qTable)
                items.Add(new QEntry { state = kv.Key, qValues = kv.Value });
        }
    }
}
