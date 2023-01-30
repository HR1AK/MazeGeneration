using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MazeSpawner : MonoBehaviour
{

    public GameObject CellPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        MazeGenerator generator = new MazeGenerator();
        MazeGeneratorCell[,] maze = generator.GenerateMaze();

        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                //Рисуем сами стенки
                //С помощью Instatiate создается игровой объект, тут - ячейка лабиринта
                Cell c =  Instantiate(CellPrefab, new Vector2(x, y), Quaternion.identity).GetComponent<Cell>();
                //SetActive(TRUE\FALSE)
                c.LeftWall.SetActive(maze[x,y].WallLeft);
                c.BottomWall.SetActive(maze[x, y].WallBottom);
            }
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log("Time = " + stopwatch.ElapsedMilliseconds);
    }
}
