using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    IDLE,
    MOVING
}

public class Player : MonoBehaviour
{
    static int[] drow = new int[] { -1, 0, 1, 0 };
    static int[] dcol = new int[] { 0, 1, 0, -1 };

    public int row { get; set; }
    public int col { get; set; }

    int initialRow;
    int initialCol;

    int currentDir = 0;
    int targetRow { get; set; }
    int targetCol { get; set; }
    Vector3 targetPos;

    public PlayerState currentState = PlayerState.IDLE;
    int movementFreedom = 0;
    float speed = 30.0f;

    Vector3 initialPosition;

    public LevelManager levelManager { get; set; }

    [SerializeField]
    LifeUI lifeUI;
    const int LIVES = 5;

    float inputFreezeTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        initialRow = row;
        initialCol = col;
        updateMovementFreedom();

        lifeUI.Lives = LIVES;
    }

    // Update is called once per frame
    void Update()
    {
        inputFreezeTime -= Time.deltaTime;
        inputFreezeTime = Mathf.Max(0, inputFreezeTime);

        switch (currentState)
        {
            case PlayerState.IDLE:
                updateIdleState();
                break;
            case PlayerState.MOVING:
                updateMovingState();
                break;
            default:
                break;
        }
    }

    void updateMovementFreedom()
    {
        movementFreedom = 1 | 1 << 1 | 1 << 2 | 1 << 3;

        for (int i = 0; i < 4; i++)
        {
            char neigh_cell = levelManager.levelData[row + drow[i], col + dcol[i]];
            if (neigh_cell == levelManager.cellTypeToCharMap[CellType.WALL])
            {
                movementFreedom ^= 1 << i;
            }
        }
    }

    void updateIdleState()
    {
        if (inputFreezeTime > 0.0f) return;

        if (Input.GetKey(KeyCode.W))
        {
            changeDirection(0);
        } 
        else if (Input.GetKey(KeyCode.D))
        {
            changeDirection(1);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            changeDirection(2);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            changeDirection(3);
        }
    }

    void changeDirection(int dir)
    {
        if ((movementFreedom & (1 << dir)) == 0) return;

        currentDir = dir;
        int current_row = row, current_col = col;
        // see how far we can go in that direction
        while (true)
        {
            // only go one cell, so the player "pauses" on each cell
            current_row += drow[dir];
            current_col += dcol[dir];
            break;

            if (levelManager.levelData[current_row, current_col] == levelManager.cellTypeToCharMap[CellType.WALL])
            {
                current_row -= drow[dir];
                current_col -= dcol[dir];
                break;
            }
        }

        currentState = PlayerState.MOVING;
        setTarget(current_row, current_col);
    }

    void setTarget(int r, int c)
    {
        targetRow = r;
        targetCol = c;

        targetPos.x = targetCol - levelManager.cols / 2.0f + .5f;
        targetPos.z = -targetRow + levelManager.rows / 2.0f - .5f;
        targetPos.y = transform.position.y;
    }

    void updateMovingState()
    {
        if (transform.position != targetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
        else
        {
            // check if we should keep going
            row = targetRow;
            col = targetCol;
            updateMovementFreedom();

            // check if on movement enhancer
            char current_cell_symbol = levelManager.levelData[row, col];
            if (MovementEnhancer.enhancers.ContainsKey(current_cell_symbol))
            {
                // apply effect
                MovementEnhancer.enhancers[current_cell_symbol].ApplyEffect(this);
                return;
            }

            // keep going in current direction
            if ((movementFreedom & (1 << currentDir)) != 0)
            {
                setTarget(row + drow[currentDir], col + dcol[currentDir]);
            } 
            else
            {
                currentState = PlayerState.IDLE;
            }
        }
    }

    public void hurt(int damage)
    {
        lifeUI.Lives -= damage;
    }

    public void kill()
    {
        transform.position = initialPosition;
        currentState = PlayerState.IDLE;
        lifeUI.Lives = LIVES;
        row = initialRow;
        col = initialCol;
        updateMovementFreedom();
        // levelManager.restart();
    }

    public void freezeInput(float _freeze_time)
    {
        inputFreezeTime = _freeze_time;
    }
}   
