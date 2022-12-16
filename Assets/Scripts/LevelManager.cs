using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public enum CellType
{
    EMPTY,
    PLAYER,
    WALL,
    SPIKE,
    DIRECTION_CHANGER,
    COIN
}

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    public Canvas pauseMenu;
    [SerializeField]
    public Canvas wonMenu;
    [SerializeField]
    private PauseComponent pauseComponent;

    public static GameMode gameMode = GameMode.EASY;
    public static int coins = 0;

    static int[] drow = new int[] { -1, 0, 1, 0 };
    static int[] dcol = new int[] { 0, 1, 0, -1 };
    const int BLOCK_SIZE = 15;
    int[,] mapsTo =
    {
        { 0, 0 },
        { 1, 0 },
        { 1, 1 },
        { 3, 0 },
        { 1, 2 },
        { 5, 0 },
        { 3, 1 },
        { 7, 0 },
        { 1, 3 },
        { 3, 3 },
        { 5, 1 },
        { 7, 3 },
        { 3, 2 },
        { 7, 2 },
        { 7, 1 },
        { 15, 0 },
    };

    [SerializeField]
    Transform playerPrefab;
    [SerializeField]
    Transform wallPrefab;
    [SerializeField]
    Transform floorPrefab;
    [SerializeField]
    Transform spikePrefab;
    [SerializeField]
    Transform directionChangerPrefab;
    [SerializeField]
    Transform coinPrefab;
    [SerializeField]
    int ROWS = 2;
    [SerializeField]
    int COLS = 2;

    Player player;

    public Dictionary<char, CellType> charToCellTypeMap = new Dictionary<char, CellType>()
    {
        { '.', CellType.EMPTY },
        { '#', CellType.WALL },
        { '$', CellType.PLAYER },
        { '!', CellType.SPIKE },
        { '+', CellType.DIRECTION_CHANGER },
        { '*', CellType.COIN },
    };

    public Dictionary<CellType, char> cellTypeToCharMap = new Dictionary<CellType, char>()
    {
        { CellType.EMPTY, '.' },
        { CellType.WALL, '#' },
        { CellType.PLAYER, '$' },
        { CellType.SPIKE, '!' },
        { CellType.DIRECTION_CHANGER, '+' },
        { CellType.COIN, '*' },
    };

    public int rows { get; set; }
    public int cols { get; set; }
    public char[,] levelData { get; set; }

    private bool isPaused = false;
    public bool IsPaused
    {
        get => isPaused;
        set
        {
            isPaused = value;
            Time.timeScale = (isPaused) ? 0 : 1;
            if (!gameEnded)
            {
                pauseMenu.enabled = isPaused;
            }
        }
    }
    private bool gameEnded = false;

    public void loadWonMenu()
    {
        Destroy(pauseComponent);
        gameEnded = true;
        IsPaused = true;
        wonMenu.GetComponentInChildren<GameWonUI>().updateData();
        wonMenu.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;

        pauseMenu.enabled = false;
        wonMenu.enabled = false;

        player = Instantiate(playerPrefab).GetComponent<Player>();
        player.levelManager = this;

        switch (gameMode)
        {
            case GameMode.EASY:
                ROWS = 2;
                COLS = 2;
                break;
            case GameMode.MEDIUM:
                ROWS = 3;
                COLS = 4;
                break;
            case GameMode.HARD:
                ROWS = 5;
                COLS = 5;
                break;
        }

        var maze = generateMaze(ROWS, COLS);
        loadLevel(buildLevelFromMaze(maze)); 
    }

    public static void levelFinished(float time)
    {
        string key = "";
        switch (gameMode)
        {
            case GameMode.EASY:
                key = "easy";
                break;
            case GameMode.MEDIUM:
                key = "medium";
                break;
            case GameMode.HARD:
                key = "hard";
                break;
        }

        float current_time = 999999.0f;
        if (PlayerPrefs.HasKey(key))
        {
            current_time = PlayerPrefs.GetFloat(key);
            if (time < current_time)
            {
                PlayerPrefs.SetFloat(key, time);
            }
        }
        else
        {
            PlayerPrefs.SetFloat(key, time);
        }
        PlayerPrefs.Save();
    }

    public void restart()
    {
        SceneManager.LoadScene("Game");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void loadLevel(char[,] level_data)
    {
        levelData = level_data;
        levelData[1, 1] = '$';

        rows = levelData.GetLength(0);
        cols = levelData.GetLength(1);

        int total_coins = 0;

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
                        obj = player.transform;
                        player.row = row;
                        player.col = col;
                        break;
                    case CellType.SPIKE:
                        obj = Instantiate(spikePrefab);
                        break;
                    case CellType.DIRECTION_CHANGER:
                        obj = Instantiate(directionChangerPrefab);
                        break;
                    case CellType.COIN:
                        total_coins += 1;
                        obj = Instantiate(coinPrefab);
                        break;
                    default:
                        break;
                }

                if (obj == null) continue;

                obj.position = new Vector3(col - cols / 2.0f + .5f, 0, -row + rows / 2.0f - .5f);
            }
        }
        Transform floor = Instantiate(floorPrefab);
        floor.localScale = new Vector3(cols, .1f, rows);
        floor.transform.position = new Vector3(0, -0.05f, 0);

        //player.coinUI.NeededCoins = total_coins;
        coins = total_coins;
    }

    int[,] generateMaze(int height, int width)
    {
        int[,] maze = new int[height, width];
        bool[,] used = new bool[height, width];
        used[0, 0] = true;
        buildMaze(0, 0, maze, used);

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
        for (int i = 0; i < 16; i++)
        {
            JToken jt = jObject[i.ToString()];
            block_data.Add(jt.ToObject<List<List<String>>>());
        }

        var random = new System.Random();
        for (int maze_row = 0; maze_row < maze_height; maze_row++)
        {
            for (int maze_col = 0; maze_col < maze_width; maze_col++)
            {
                int maps_to = mapsTo[maze[maze_row, maze_col], 0];
                int rotation = mapsTo[maze[maze_row, maze_col], 1];

                int configurations = block_data.ElementAt(maps_to).Count;
                int idx = random.Next(configurations);

                var block = block_data.ElementAt(maps_to).ElementAt(idx);
                char[,] char_block = getCharBlock(block);

                for (int i = 0; i < rotation; i++)
                {
                    char_block = rotateBlock(char_block);
                }
                for (int row = 0; row < BLOCK_SIZE; row++)
                {
                    for (int col = 0; col < BLOCK_SIZE; col++)
                    {
                        ret[row + maze_row * BLOCK_SIZE, col + maze_col * BLOCK_SIZE] = char_block[row, col];
                    }
                }
            }
        }

        return ret;
    }

    char[,] getCharBlock(List<String> block)
    {
        char[,] ret = new char[block.Count, block[0].Length];

        int row = 0, col = 0;
        foreach (String s in block)
        {
            col = 0;
            foreach (char c in s)
            {
                ret[row, col] = c;
                col += 1;
            }
            row += 1;
        }

        return ret;
    }

    char[,] rotateBlock(char[,] block)
    {
        char[,] ret = new char[block.GetLength(0), block.GetLength(1)];

        for (int row = 0; row < block.GetLength(0); row++)
        {
            for (int col = 0; col < block.GetLength(1); col++)
            {
                ret[col, block.GetLength(1) - row - 1] = block[row, col]; 
            }
        }

        return ret;
    }

    private void buildMaze(int row, int col, int[,] maze, bool[,] used)
    {
        int rows = maze.GetLength(0);
        int cols = maze.GetLength(1);

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
            buildMaze(neigh_row_2, neigh_col_2, maze, used);
        }
    }
}
