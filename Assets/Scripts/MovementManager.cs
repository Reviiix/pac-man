using System;
using System.Collections;
using System.Linq;
using Abstractions;
using GridSystem.GridItems;
using UnityEngine;
using UnityEngine.Serialization;

public class MovementManager : Singleton<MovementManager>
{
    //job system works better with collections than single objects
    [SerializeField] private Transform[] objectsToMove;
    //0 = player
    //1 = enemy
    [Header(nameof(player))]
    [SerializeField] private Transform player;
    [FormerlySerializedAs("speed")] [SerializeField] private int playerSpeed;
    [FormerlySerializedAs("currentPosition")] [SerializeField] private GridItem currentPlayerPosition;
    [FormerlySerializedAs("nextPosition")] [SerializeField] private GridItem nextPlayerPosition;
    private readonly WaitUntil waitUntilPlayerHasReachedCurrentTarget = new (() => !_moving);
    private bool canMove;
    private static bool _moving;
    private const float NewPositionDistanceTolerance = 0.01f;
    private const int CurrentPositionDistanceTolerance = 10;
    private Coroutine directionChange;
    public Direction currentDirection;
    [Header("Enemies")]
    [SerializeField] private Transform one;
    
    public void Initialise()
    {
        SetNextPosition(GetNextPosition(currentDirection));
        canMove = true;
    }

    private void Update()
    {
        if (!canMove) return;
        CheckForInput();
        MovePlayer();
    }

    private void CheckForInput()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            DetermineDirectionChange(KeyCode.W);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            DetermineDirectionChange(KeyCode.S);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            DetermineDirectionChange(KeyCode.A);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            DetermineDirectionChange(KeyCode.D);
        }
    }
    
    private void MovePlayer()
    {
        if (nextPlayerPosition is Wall) return;
        var targetDestination = nextPlayerPosition.transform.position;
        _moving = true;
        MovementJobs.MoveObjects(objectsToMove, new [] {Vector3.MoveTowards(player.position, targetDestination, playerSpeed * Time.deltaTime)});
        if (NewPositionReached())
        {
            OnDestinationReached();
        }
    }
    
    private void OnDestinationReached()
    {
        SetCurrentPosition(nextPlayerPosition);
        MovePlayerToCurrentGridItem();
        SetNextPosition(GetNextPosition(currentDirection));
        _moving = false;
    }

    private void MovePlayerToCurrentGridItem()
    {
        MovementJobs.MoveObjects(objectsToMove, new [] {Vector3.MoveTowards(player.transform.position, currentPlayerPosition.transform.position, playerSpeed * Time.deltaTime)});
    }

    private void DetermineDirectionChange(KeyCode key)
    {
        if (directionChange != null)
        {
            StopCoroutine(directionChange);
        }
        
        if (CloseToCurrentGridItem())
        {
            ChangeDirection(key);
        }
        else
        {
            directionChange = StartCoroutine(WaitUntilPlayerHasReachedTarget(()=>ChangeDirection(key)));
        }
    }

    private void ChangeDirection(KeyCode key)
    {
        var direction = GetDirectionFromKeyCode(key);
        if (currentPlayerPosition.GetAdjacentItem(direction) is Wall) return;
        currentDirection = direction;
        SetCurrentPosition(currentPlayerPosition);
        SetNextPosition(currentPlayerPosition.GetAdjacentItem(currentDirection));
    }

    private Direction GetDirectionFromKeyCode(KeyCode key)
    {
        return key switch
        {
            KeyCode.W => Direction.Up,
            KeyCode.A => Direction.Left,
            KeyCode.S => Direction.Down,
            KeyCode.D => Direction.Right,
            _ => currentDirection
        };
    }

    private bool CloseToCurrentGridItem()
    {
        return Vector3.Distance(player.position, currentPlayerPosition.transform.position) < CurrentPositionDistanceTolerance;
    }

    private bool NewPositionReached()
    {
        return Vector3.Distance(player.transform.position, nextPlayerPosition.transform.position) < NewPositionDistanceTolerance;
    }

    private void SetCurrentPosition(GridItem position)
    {
        currentPlayerPosition = position;
    }
    
    private void SetNextPosition(GridItem position)
    {
        nextPlayerPosition = position;
    }

    private GridItem GetNextPosition(Direction direction)
    {
        return nextPlayerPosition.GetAdjacentItem(direction);
    }
    
    private IEnumerator WaitUntilPlayerHasReachedTarget(Action callBack)
    {
        yield return waitUntilPlayerHasReachedCurrentTarget;
        callBack();
    }
}

public enum Direction
{
    Up, Down, Left, Right
}
