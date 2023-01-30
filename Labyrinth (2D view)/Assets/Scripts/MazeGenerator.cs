using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

//Класс, отвечающий за одну ячейку  //The class responsible for one mesh
public class MazeGeneratorCell
{
    public int X;
    public int Y;

    public bool WallLeft = true;
    public bool WallBottom = true;

    public bool Visited = false;
    public int DistanceFromStart;

    public int group = 0;
}


public class MazeGenerator
{
    //Размер лабиринта //Size of Maze
    public int Width = 500;
    public int Height = 500;

    //maze creation method
    public MazeGeneratorCell[,] GenerateMaze()
    {
        MazeGeneratorCell[,] maze = new MazeGeneratorCell[Width, Height];

        //Заполняем сетку ячейками 
        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                maze[x, y] = new MazeGeneratorCell { X = x, Y = y };
            }
        }

        //Убираем "неровные края сверху и справа"
        for (int x = 0; x < maze.GetLength(0); x++)
            maze[x, maze.GetLength(1) - 1].WallLeft = false;

        for (int y = 0; y < maze.GetLength(1); y++)
            maze[maze.GetLength(0) - 1, y].WallBottom = false;

        //вызов алгоритма рекурсивного возврата
        RemoveWallsWithBackTracker(maze);

        //вызов генерация выхода
        PlaceMazeExit(maze);

        //возвращаем отрисаованный лабиринт
        return maze;
    }

    private void RemoveWallsWithBackTracker(MazeGeneratorCell[,] maze)
    {
        MazeGeneratorCell current = maze[0,0];
        current.Visited = true;
        current.DistanceFromStart = 0;

        //cоздаем стек
        Stack<MazeGeneratorCell> stack = new Stack<MazeGeneratorCell>();

        do
        {
            List<MazeGeneratorCell> UnvisitedCells = new List<MazeGeneratorCell>();

            if (current.X > 0 && !maze[current.X - 1, current.Y].Visited) UnvisitedCells.Add(maze[current.X - 1, current.Y]);
            if (current.Y > 0 && !maze[current.X, current.Y-1].Visited) UnvisitedCells.Add(maze[current.X, current.Y - 1]);
            if (current.X < Width - 2 && !maze[current.X + 1, current.Y].Visited) UnvisitedCells.Add(maze[current.X + 1, current.Y]);
            if (current.Y < Height - 2 && !maze[current.X, current.Y + 1].Visited) UnvisitedCells.Add(maze[current.X, current.Y + 1]);

            if(UnvisitedCells.Count > 0)
            {
                MazeGeneratorCell chosen = UnvisitedCells[UnityEngine.Random.Range(0, UnvisitedCells.Count)];
                RemoveWall(current, chosen);

                chosen.Visited = true;
                current = chosen;
                stack.Push(chosen);
                chosen.DistanceFromStart = stack.Count;

            }
            else
            {
                current = stack.Pop();
            }
        }
        while (stack.Count > 0);
    }

    private void RemoveWall(MazeGeneratorCell a, MazeGeneratorCell b)
    {
        if (a.X == b.X)
        {
            if (a.Y > b.Y)
            {
                a.WallBottom = false;
            }
            else
            {
                b.WallBottom = false;
            }
        }
        else
        {
            if (a.X > b.X)
            {
                a.WallLeft = false;
            }
            else
            {
                b.WallLeft = false;
            }
        }

    }

    //Генерация выхода
    private void PlaceMazeExit(MazeGeneratorCell[,] maze)
    {
        MazeGeneratorCell furthest = maze[0,0];

        for (int x = 0; x<maze.GetLength(0); x++)
        {
            if (maze[x, Height - 2].DistanceFromStart > furthest.DistanceFromStart)
                furthest = maze[x, Height - 2];

            if (maze[x, 0].DistanceFromStart > furthest.DistanceFromStart)
                furthest = maze[x, 0];
        }

        for (int y = 0; y < maze.GetLength(1); y++)
        {
            if (maze[0, y].DistanceFromStart > furthest.DistanceFromStart)
                furthest = maze[0, y];

            if (maze[Width - 2, y].DistanceFromStart > furthest.DistanceFromStart)
                furthest = maze[Width - 2, 0];
        }

        if (furthest.X == 0)
            furthest.WallLeft = false;
        else
        {
            if (furthest.Y == 0)
            {
                furthest.WallBottom = false;
            }
            else
            {
                if (furthest.X == Width - 2)
                    maze[furthest.X+1, furthest.Y].WallLeft = false;
                else
                {
                    maze[furthest.X, furthest.Y+1].WallBottom = false;
                }

            }
            
        }

    }
}

public class MazeGeneratorEller
{
    public int Width = 30;
    public int Height = 20;


    public MazeGeneratorCell[,] GenerateMaze()
    {
        MazeGeneratorCell[,] maze = new MazeGeneratorCell[Width, Height];

        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                maze[x, y] = new MazeGeneratorCell { X = x, Y = y, WallBottom = false, WallLeft = false};

                if ((x == 0 || x == maze.GetLength(0) - 1) && y != maze.GetLength(1) - 1)
                {
                    maze[x, y].WallLeft = true;
                }

                if ((y == 0 || y == maze.GetLength(1) - 1) && x != maze.GetLength(0) - 1)
                {
                    maze[x, y].WallBottom = true;
                }
            }
        }

        EllerAlgorithm(maze);

        return maze;
    }

    public void EllerAlgorithm(MazeGeneratorCell[,] maze)
    {
        int count_group = 1;
        int i = 0;
        int current_group;

        for (int y = maze.GetLength(1) - 2; y > 0; y--)
        {
            for (int x = 0; x < maze.GetLength(0) - 1; x++)
            {
                if (maze[x, y].group == 0)
                {
                    maze[x, y].group = count_group;
                    count_group++;
                }
            }

            for (int x = 0; x < maze.GetLength(0) - 2; x++)
            {

                

                if (UnityEngine.Random.Range(0, 2) == 1 || maze[x, y].group == maze[x+1, y].group)
                {
                    maze[x + 1, y].WallLeft = true;
                }
                else
                {
                    i = x;
                    current_group = maze[x + 1, y].group;
                    while (maze[i + 1, y].group == current_group)
                    {
                        maze[i + 1, y].group = maze[i, y].group;
                        i++;
                    }
                }

                if (UnityEngine.Random.Range(0, 2) == 1)
                {
                    maze[x, y].WallBottom = true;
                }
            }

            bool flag = false;
            int count = 1;

            for (int x = 0; x < maze.GetLength(0) - 1; x++)
            {
                if (maze[x, y].group == maze[x + 1, y].group)
                {
                    count++;

                    if (maze[x, y].WallBottom == false)
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (count == 1)
                    {
                        maze[x, y].WallBottom = false;
                        count_group++;
                        maze[x, y - 1].group = count_group;
                    }
                    else
                    {
                        if (flag == false)
                        {
                            maze[x, y].WallBottom = false;
                            count_group++;
                            maze[x, y - 1].group = count_group;
                        }


                    }

                    count = 1;
                    flag = false;
                }
            }


            //копируем на строку ниже
            for (int x = 0; x < maze.GetLength(0) - 1; x++)
            {
                if (y != 1)
                {
                    maze[x, y - 1].WallBottom = maze[x, y].WallBottom;
                }

                maze[x, y - 1].WallLeft = maze[x, y].WallLeft;
                maze[x, y - 1].group = maze[x, y].group;
            }


            for (int x = 0; x < maze.GetLength(0) - 1; x++)
            {

                if (x != 0)
                {
                    maze[x, y - 1].WallLeft = false;
                }

                if (maze[x, y - 1].WallBottom == true)
                {
                    maze[x, y - 1].WallBottom = false;
                    count_group++;
                    maze[x, y - 1].group = count_group;
                }

            }



        }
        int e = 0;
        for (int x = 0; x < maze.GetLength(0) - 2; x++)
        {

            if (maze[x, e].group == maze[x + 1, e].group)
            {
                maze[x+1, e].WallLeft = true;
            }
            else
            {
                maze[x+1, e].WallLeft = false;
                maze[x+1, e].group = maze[x, e].group;
            }
        }
        
    }
}

public class MazeGeneratorAldousBroder
{
    public int Width = 100;
    public int Height = 100;

    public MazeGeneratorCell[,] GenerateMaze()
    {
        MazeGeneratorCell[,] maze = new MazeGeneratorCell[Width, Height];

        //Заполняем сетку ячейками 
        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                maze[x, y] = new MazeGeneratorCell { X = x, Y = y };
            }
        }

        //Убираем "неровные края сверху и справа"
        for (int x = 0; x < maze.GetLength(0); x++)
            maze[x, maze.GetLength(1) - 1].WallLeft = false;

        for (int y = 0; y < maze.GetLength(1); y++)
            maze[maze.GetLength(0) - 1, y].WallBottom = false;

        //вызов алгоритма рекурсивного возврата
        AldousBroderAlgorithm(maze);

        //выход
        maze[maze.GetLength(0)-1, maze.GetLength(1) - 2].WallLeft = false;

        //возвращаем отрисаованный лабиринт
        return maze;
    }

    public void AldousBroderAlgorithm(MazeGeneratorCell[,] maze)
    {
        int count = (Height-1) * (Width-1);

        MazeGeneratorCell current = maze[0, 0];
        current.Visited = true;
        count--;

        do
        {
            List<MazeGeneratorCell> UnvisitedCells = new List<MazeGeneratorCell>();
            if (current.X > 0) UnvisitedCells.Add(maze[current.X - 1, current.Y]);
            if (current.Y > 0) UnvisitedCells.Add(maze[current.X, current.Y - 1]);
            if (current.X < Width - 2) UnvisitedCells.Add(maze[current.X + 1, current.Y]);
            if (current.Y < Height - 2) UnvisitedCells.Add(maze[current.X, current.Y + 1]);

            
            MazeGeneratorCell chosen = UnvisitedCells[UnityEngine.Random.Range(0, UnvisitedCells.Count)];
            if (chosen.Visited == false)
            {
                RemoveWall(current, chosen);
               
                count--;
            }

            chosen.Visited = true;
            current = chosen;

        }
        while(count != 0);
    }

    private void RemoveWall(MazeGeneratorCell a, MazeGeneratorCell b)
    {
        if (a.X == b.X)
        {
            if (a.Y > b.Y)
            {
                a.WallBottom = false;
            }
            else
            {
                b.WallBottom = false;
            }
        }
        else
        {
            if (a.X > b.X)
            {
                a.WallLeft = false;
            }
            else
            {
                b.WallLeft = false;
            }
        }

    }
}

public class MazeGeneratorPrim
{
    public int Width = 100;
    public int Height = 100;

    public MazeGeneratorCell[,] GenerateMaze()
    {
        MazeGeneratorCell[,] maze = new MazeGeneratorCell[Width, Height];

        //Заполняем сетку ячейками 
        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                maze[x, y] = new MazeGeneratorCell { X = x, Y = y };
            }
        }

        //Убираем "неровные края сверху и справа"
        for (int x = 0; x < maze.GetLength(0); x++)
            maze[x, maze.GetLength(1) - 1].WallLeft = false;

        for (int y = 0; y < maze.GetLength(1); y++)
            maze[maze.GetLength(0) - 1, y].WallBottom = false;

        //вызов алгоритма рекурсивного возврата
        PrimsAlgorithm(maze);

        //выход
        maze[maze.GetLength(0) - 1, maze.GetLength(1) - 2].WallLeft = false;

        //возвращаем отрисаованный лабиринт
        return maze;
    }

    private void PrimsAlgorithm(MazeGeneratorCell[,] maze)
    {
        List<MazeGeneratorCell> Cells = new List<MazeGeneratorCell>();

        MazeGeneratorCell current = maze[0, 0];
        current.Visited = true;
        Cells.Add(maze[0, 0]);

        do
        {
            current = Cells[UnityEngine.Random.Range(0, Cells.Count - 1)];
            List<MazeGeneratorCell> UnvisitedCells = new List<MazeGeneratorCell>();
            if (current.X > 0 && !maze[current.X - 1, current.Y].Visited) UnvisitedCells.Add(maze[current.X - 1, current.Y]);
            if (current.Y > 0 && !maze[current.X, current.Y - 1].Visited) UnvisitedCells.Add(maze[current.X, current.Y - 1]);
            if (current.X < Width - 2 && !maze[current.X + 1, current.Y].Visited) UnvisitedCells.Add(maze[current.X + 1, current.Y]);
            if (current.Y < Height - 2 && !maze[current.X, current.Y + 1].Visited) UnvisitedCells.Add(maze[current.X, current.Y + 1]);

            if (UnvisitedCells.Count == 0)
            {
                Cells.Remove(current);
            }
            else
            {
                MazeGeneratorCell chosen = UnvisitedCells[UnityEngine.Random.Range(0, UnvisitedCells.Count)];
                RemoveWall(current, chosen);
                chosen.Visited = true;
                Cells.Add(chosen);
            }
        }
        while (Cells.Count != 0);
    }

    private void RemoveWall(MazeGeneratorCell a, MazeGeneratorCell b)
    {
        if (a.X == b.X)
        {
            if (a.Y > b.Y)
            {
                a.WallBottom = false;
            }
            else
            {
                b.WallBottom = false;
            }
        }
        else
        {
            if (a.X > b.X)
            {
                a.WallLeft = false;
            }
            else
            {
                b.WallLeft = false;
            }
        }

    }
}


