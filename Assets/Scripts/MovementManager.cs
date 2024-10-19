using System;
using System.Collections;
using System.Linq;
using Abstractions;
using GridSystem;
using GridSystem.GridItems;
using UnityEngine;

public class MovementManager : Singleton<MovementManager>
{
    [SerializeField] private int speed;
    [SerializeField] private Transform[] player;
    [SerializeField] private GridItem currentPosition;
    [SerializeField] private GridItem nextPosition;
    private readonly WaitUntil waitUntilPlayerHasReachedCurrentTarget = new (() => !_moving);
    private bool canMove;
    private static bool _moving;
    private const float NewPositionDistanceTolerance = 0.01f;
    private const int CurrentPositionDistanceTolerance = 10;
    private Coroutine directionChange;
    public Direction currentDirection;


    public void Initialise()
    {
        SetNextPosition(GetNextPosition(currentDirection));
        canMove = true;
    }

    private IEnumerator WaitUntilPlayerHasReachedTarget(Action callBack)
    {
        yield return waitUntilPlayerHasReachedCurrentTarget;
        callBack();
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
        if (currentPosition.GetAdjacentItem(direction) is Wall) return;
        currentDirection = direction;
        SetCurrentPosition(currentPosition);
        SetNextPosition(currentPosition.GetAdjacentItem(currentDirection));
    }

    private Direction GetDirectionFromKeyCode(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.W:
                return Direction.Up;
            case KeyCode.A:
                return Direction.Left;
            case KeyCode.S:
                return Direction.Down;
            case KeyCode.D:
                return Direction.Right;
        }

        return currentDirection;
    }
    
    private void MovePlayer()
    {
        if (nextPosition is Wall) return;
        var targetDestination = nextPosition.transform.position;
        _moving = true;
        MovementJobs.MoveObjects(player, new [] {Vector3.MoveTowards(player.FirstOrDefault().transform.position, targetDestination, speed * Time.deltaTime)});
        if (NewPositionReached())
        {
            OnDestinationReached();
        }
    }

    private bool CloseToCurrentGridItem()
    {
        return Vector3.Distance(player.FirstOrDefault().transform.position, currentPosition.transform.position) < CurrentPositionDistanceTolerance;
    }

    private bool NewPositionReached()
    {
        return Vector3.Distance(player.FirstOrDefault().transform.position, nextPosition.transform.position) < NewPositionDistanceTolerance;
    }

    private void OnDestinationReached()
    {
        SetCurrentPosition(nextPosition);
        MovePlayerToCurrentGridItem();
        SetNextPosition(GetNextPosition(currentDirection));
        _moving = false;
    }

    private void MovePlayerToCurrentGridItem()
    {
        MovementJobs.MoveObjects(player, new [] {Vector3.MoveTowards(player.FirstOrDefault().transform.position, currentPosition.transform.position, speed * Time.deltaTime)});
    }

    private void SetCurrentPosition(GridItem position)
    {
        currentPosition = position;
    }
    
    private void SetNextPosition(GridItem position)
    {
        nextPosition = position;
    }

    private GridItem GetNextPosition(Direction direction)
    {
        return nextPosition.GetAdjacentItem(direction);
    }
}

public enum Direction
{
    Up, Down, Left, Right
}
