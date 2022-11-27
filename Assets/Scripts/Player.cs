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

    int targetRow { get; set; }
    int targetCol { get; set; }
    Vector3 targetPos;

    PlayerState currentState = PlayerState.IDLE;
    int movementFreedom = 0;
    float speed = 30.0f;

    Vector3 initialPosition;

    public LevelManager levelManager { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        initialRow = row;
        initialCol = col;
        updateMovementFreedom();
    }

    // Update is called once per frame
    void Update()
    {
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

        int current_row = row, current_col = col;
        bool should_stop = false;
        while (!should_stop)
        {
            current_row += drow[dir];
            current_col += dcol[dir];

            if (levelManager.levelData[current_row, current_col] == levelManager.cellTypeToCharMap[CellType.WALL])
            {
                current_row -= drow[dir];
                current_col -= dcol[dir];
                should_stop = true;
            }
        }

        currentState = PlayerState.MOVING;
        targetRow = current_row;
        targetCol = current_col;

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
            currentState = PlayerState.IDLE;
            row = targetRow;
            col = targetCol;
            updateMovementFreedom();
        }
    }

    public void kill()
    {
        transform.position = initialPosition;
        currentState = PlayerState.IDLE;
        row = initialRow;
        col = initialCol;
        updateMovementFreedom();
        // levelManager.restart();
    }
}   
