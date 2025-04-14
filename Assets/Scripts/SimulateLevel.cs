using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SimulateLevel 
{
    private char[,] grid;
    private Vector2Int frogPos;
    private Vector2Int playerPos;
    private Vector2Int goalPos;
    private HashSet<Vector2Int> crocTiles = new();
    private Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    public static char[,] Parse(string levelText)
    {
        string[] lines = levelText.Trim().Split('\n');
        int height = lines.Length;
        int width = lines[0].Length;
        char[,] grid = new char[width, height];

        for (int y = 0; y < height; y++)
        {
            string line = lines[height - 1 - y];
            for (int x = 0; x < width; x++)
            {
                if (x < line.Length)
                    grid[x, y] = line[x];
                else
                    grid[x, y] = ' ';
            }
        }

        return grid;
    }

    public SimulateLevel(char[,] inputGrid)
    {
        grid = (char[,])inputGrid.Clone();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                char tile = grid[x, y];
                Vector2Int pos = new(x, y);

                if (tile == '1') playerPos = pos;
                if (tile == '2') frogPos = pos;
                if (tile == 'F') goalPos = pos;
                if (tile == 'C') crocTiles.Add(pos);
                if (tile == 'I')
                {
                    crocTiles.Add(pos);
                    grid[x, y] = 'B'; 
                }
            }
        }
    }

    public string GetState()
    {
        return $"{frogPos.x},{frogPos.y},{playerPos.x},{playerPos.y}";
    }

    public bool Step(Vector2Int dir, out float reward, out bool done)
    {
        Vector2Int target = frogPos + dir;

        if (!IsInBounds(target))
        {
            reward = -10f;
            done = false;
            return false;
        }

        if (target == playerPos)
        {
            reward = -20f;
            done = false;
            return false;
        }

        char tile = grid[target.x, target.y];

        if (tile == '#' || tile == ' ')
        {
            reward = -20f;
            done = false;
            return false;
        }

        if (tile == 'D')
        {
            reward = -10f;
            done = false;
            return false;
        }
        else if (tile == 'B')
        {
            Vector2Int pushTarget = target + dir;
            if (IsInBounds(pushTarget) && grid[pushTarget.x, pushTarget.y] == 'D')
            {
                grid[pushTarget.x, pushTarget.y] = '-'; 
                grid[target.x, target.y] = '-';         
                reward = 200f;
                done = false;
                return false;
            }
            else
            {
                reward = -10f;
                done = false;
                return false;
            }
        }
        else
        {
            reward = -1f;
        }

        if (crocTiles.Contains(frogPos))
        {
            grid[frogPos.x, frogPos.y] = ' ';
            crocTiles.Remove(frogPos);
        }

        frogPos = target;
        done = frogPos == goalPos;

        if (done)
        {
            reward = 100f;
        }

        return true;
    }

    public void MovePlayerTowardGoalWithEpsilon(float epsilon = 0.2f)
    {
        if (Random.value <= epsilon)
        {
            MovePlayerRandom();
        }
        else
        {
            MovePlayerTowardGoal();
        }
    }

    private void MovePlayerTowardGoal()
    {
        Vector2Int bestDir = Vector2Int.zero;
        float bestDist = float.MaxValue;

        foreach (var dir in directions)
        {
            Vector2Int target = playerPos + dir;
            if (!IsInBounds(target)) continue;

            char tile = grid[target.x, target.y];
            if (tile != '-' && tile != 'C' && tile != 'F') continue;

            float dist = Vector2Int.Distance(target, goalPos);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestDir = dir;
            }
        }

        if (bestDir != Vector2Int.zero)
        {
            if (crocTiles.Contains(playerPos))
            {
                grid[playerPos.x, playerPos.y] = ' ';
                crocTiles.Remove(playerPos);
            }
            playerPos += bestDir;
        }
    }

    private void MovePlayerRandom()
    {
        List<Vector2Int> validMoves = new();

        foreach (var dir in directions)
        {
            Vector2Int target = playerPos + dir;
            if (!IsInBounds(target)) continue;

            char tile = grid[target.x, target.y];
            if (tile == '-' || tile == 'C' || tile == 'F')
                validMoves.Add(dir);
        }

        if (validMoves.Count > 0)
        {
            Vector2Int chosen = validMoves[Random.Range(0, validMoves.Count)];

            if (crocTiles.Contains(playerPos))
            {
                grid[playerPos.x, playerPos.y] = ' ';
                crocTiles.Remove(playerPos);
            }
            playerPos += chosen;
        }
    }

    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < grid.GetLength(0) &&
               pos.y >= 0 && pos.y < grid.GetLength(1);
    }

    private bool IsStandingOnCroc(Vector2Int pos)
    {
        return crocTiles.Contains(pos);
    }
}
