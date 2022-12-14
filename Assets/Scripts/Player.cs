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

    private PlayerState currentState;
    public PlayerState CurrentState
    {
        get => currentState;
        set
        {
            currentState = value;

            switch (value)
            {
                case PlayerState.IDLE:
                    stillBar.fadeIn();
                    stillBar.startFilling();
                    break;
                case PlayerState.MOVING:
                    stillBar.fadeOut();
                    stillBar.stopFilling();
                    break;
            }
        }
    }

    int movementFreedom = 0;
    float speed = 30.0f;

    Vector3 initialPosition;

    public LevelManager levelManager { get; set; }

    [SerializeField]
    LifeUI lifeUI;
    [SerializeField]
    public CoinUI coinUI;
    [SerializeField]
    ProgressBar stillBar;
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
        CurrentState = PlayerState.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        inputFreezeTime -= Time.deltaTime;
        inputFreezeTime = Mathf.Max(0, inputFreezeTime);

        switch (CurrentState)
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

        if (stillBar.isFilled())
        {
            kill();
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

        // only go one cell, so the player "pauses" on each cell
        current_row += drow[dir];
        current_col += dcol[dir];

        CurrentState = PlayerState.MOVING;
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
                CurrentState = PlayerState.IDLE;
            }
        }
    }

    public void hurt(int damage)
    {
        lifeUI.Lives -= damage;
    }

    public void kill()
    {
        //stillBar.reset();
        //freezeInput(.1f);
        //transform.position = initialPosition;
        //CurrentState = PlayerState.IDLE;
        //lifeUI.Lives = LIVES;
        //row = initialRow;
        //col = initialCol;
        //updateMovementFreedom();
         levelManager.restart();
    }

    public void pickCoin()
    {
        coinUI.collectCoin();
    }
    public void freezeInput(float _freeze_time)
    {
        inputFreezeTime = _freeze_time;
    }
}   
