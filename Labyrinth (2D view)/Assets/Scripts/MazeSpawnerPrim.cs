using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MazeSpawnerPrim : MonoBehaviour
{
    public GameObject CellPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        MazeGeneratorPrim generator = new MazeGeneratorPrim();
        MazeGeneratorCell[,] maze = generator.GenerateMaze();

        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                //������ ���� ������
                //� ������� Instatiate ��������� ������� ������, ��� - ������ ���������
                Cell c = Instantiate(CellPrefab, new Vector2(x, y), Quaternion.identity).GetComponent<Cell>();
                //SetActive(TRUE\FALSE)
                c.LeftWall.SetActive(maze[x, y].WallLeft);
                c.BottomWall.SetActive(maze[x, y].WallBottom);
            }
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds);
        //Console.WriteLine("����� = " + stopwatch);
        
    }
}
