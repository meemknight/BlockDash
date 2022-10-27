using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    EMPTY,
    PLAYER,
    WALL
}

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    Transform playerPrefab;
    [SerializeField]
    Transform wallPrefab;
    [SerializeField]
    Transform FloorPrefab;

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
    public List<List<char>> levelData { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        List<List<char>> level_data = new List<List<char>> {
                new List<char> { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                new List<char> { '#', '.', '#', '.', '.', '.', '.', '$', '.', '#' },
                new List<char> { '#', '.', '.', '.', '.', '.', '.', '#', '.', '#' },
                new List<char> { '#', '.', '#', '#', '#', '#', '#', '#', '.', '#' },
                new List<char> { '#', '.', '.', '.', '.', '.', '.', '.', '.', '#' },
                new List<char> { '#', '#', '.', '.', '.', '#', '#', '.', '.', '#' },
                new List<char> { '#', '.', '.', '#', '.', '.', '#', '.', '.', '#' },
                new List<char> { '#', '.', '#', '#', '.', '.', '#', '.', '.', '#' },
                new List<char> { '#', '.', '.', '.', '.', '.', '.', '.', '.', '#' },
                new List<char> { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
            };
        
        loadLevel(level_data);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void loadLevel(List<List<char>> level_data)
    {
        levelData = level_data;

        cols = levelData.Count;
        rows = levelData[0].Count;

        for (int row = 0; row < rows; row++)
        { 
            for (int col = 0; col < cols; col ++)
            {
                char cell = levelData[row][col];
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

                obj.position = new Vector3(col - rows / 2 + .5f, 0, -row + cols / 2 - .5f);
            }
        }
    }
}
