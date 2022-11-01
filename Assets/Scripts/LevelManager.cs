using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public enum CellType
{
    EMPTY,
    PLAYER,
    WALL
}

public class LevelManager : MonoBehaviour
{
    static int[] drow = new int[] { -1, 0, 1, 0 };
    static int[] dcol = new int[] { 0, 1, 0, -1 };
    const int BLOCK_SIZE = 9;

    [SerializeField]
    Transform playerPrefab;
    [SerializeField]
    Transform wallPrefab;
    [SerializeField]
    Transform floorPrefab;

    public Dictionary<char, CellType> charToCellTypeMap = new Dictionary<char, CellType>()
    {
        { '.', CellType.EMPTY },
        { '#', CellType.WALL },
        { '$', CellType.PLAYER }
    };

    public Dictionary<CellType, char> cellTypeToCharMap = new Dictionary<CellType, char>()
    {
        { CellType.EMPTY, '.' },
        { CellType.WALL, '#' },
        { CellType.PLAYER, '$' }
    };

    public int rows { get; set; }
    public int cols { get; set; }
    public char[,] levelData { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        char[,] level_data = new char[9, 9]{
            { '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            { '#', '#', '#', '#', '#', '#', '#', '#', '#'}
        };

        // loadLevel(level_data);

        var maze = generateMaze(4, 4);
        loadLevel(buildLevelFromMaze(maze)); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void loadLevel(char[,] level_data)
    {
        levelData = level_data;
        level_data[1, 1] = '$';

        rows = levelData.GetLength(0);
        cols = levelData.GetLength(1);
        Debug.Log(rows + " " + cols);

        for (int row = 0; row < rows; row++)
        { 
            for (int col = 0; col < cols; col ++)
            {
                char cell = levelData[row, col];
                Transform obj = null;
                switch (charToCellTypeMap[cell])
                {
                    case CellType.EMPTY:
                        break;
                    case CellType.WALL:
                        obj = Instantiate(wallPrefab);
                        break;
                    case CellType.PLAYER:
                        obj = Instantiate(playerPrefab);
                        Player player = obj.GetComponent<Player>();
                        player.levelManager = this;
                        player.row = row;
                        player.col = col;
                        break;
                    default:
                        break;
                }

                if (obj == null) continue;

                obj.position = new Vector3(col - rows / 2.0f + .5f, 0, -row + cols / 2.0f - .5f);
            }
        }
        Transform floor = Instantiate(floorPrefab);
        floor.localScale = new Vector3(cols, .1f, rows);
    }

    int[,] generateMaze(int height, int width)
    {
        int[,] freeCells = new int[height, width];
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
                freeCells[row, col] = 1 | 1 << 1 | 1 << 2 | 1 << 3;
        }
        // for the edges, xor them to mark that it can not move there
        for (int col = 0; col < width; col++)
        {
            freeCells[0, col] ^= 1;
            freeCells[height - 1, col] ^= 1 << 2;
        }
        for (int row = 0; row < height; row++)
        {
            freeCells[row, 0] ^= 1 << 3;
            freeCells[row, width-1] ^= 1 << 1;
        }

        int[,] maze = new int[height, width];
        bool[,] used = new bool[height, width];
        used[0, 0] = true;
        buildMaze(0, 0, maze, freeCells, used);

        return maze;
    }

    private char[,] buildLevelFromMaze(int[,] maze)
    {
        int maze_height = maze.GetLength(0);
        int maze_width = maze.GetLength(1);
        char[,] ret = new char[maze_height * BLOCK_SIZE, maze_width * BLOCK_SIZE];

        String json_block_data = File.ReadAllText("Assets/GameData/BlockData.json");

        List<List<List<String>>> block_data = new List<List<List<String>>>();

        JObject jObject = JObject.Parse(json_block_data);
        for (int i = 1; i < 16; i++)
        {
            JToken jt = jObject[i.ToString()];
            block_data.Add(jt.ToObject<List<List<String>>>());
        }

        var random = new System.Random();
        for (int maze_row = 0; maze_row < maze_height; maze_row++)
        {
            for (int maze_col = 0; maze_col < maze_width; maze_col++)
            {
                int configurations = block_data.ElementAt(maze[maze_row, maze_col] - 1).Count;
                int idx = random.Next(configurations);

                var block = block_data.ElementAt(maze[maze_row, maze_col] - 1).ElementAt(idx);
                for (int row = 0; row < BLOCK_SIZE; row++)
                {
                    for (int col = 0; col < BLOCK_SIZE; col++)
                    {
                        ret[row + maze_row * BLOCK_SIZE, col + maze_col * BLOCK_SIZE] = block.ElementAt(row)[col];
                    }
                }
            }
        }

        return ret;
    }

    private void buildMaze(int row, int col, int[,] maze, int[,] freeCells, bool[,] used)
    {
        int rows = freeCells.GetLength(0);
        int cols = freeCells.GetLength(1);

        while (true)
        {
            var random = new System.Random();
            List<int> availableDirections = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                int neigh_row = row + drow[i], neigh_col = col + dcol[i];
                if (neigh_row < 0 || neigh_row >= rows || neigh_col < 0 || neigh_col >= cols) continue;
                if (!used[neigh_row, neigh_col])
                {
                    availableDirections.Add(i);
                }
            }
            if (availableDirections.Count == 0) break;

            int direction = availableDirections[random.Next(availableDirections.Count)];
            int opposite_direction = (direction + 2) % 4;
            int neigh_row_2 = row + drow[direction], neigh_col_2 = col + dcol[direction];

            maze[row, col] |= 1 << direction;
            maze[neigh_row_2, neigh_col_2] |= 1 << opposite_direction;

            used[neigh_row_2, neigh_col_2] = true;
            buildMaze(neigh_row_2, neigh_col_2, maze, freeCells, used);
        }
    }
}
