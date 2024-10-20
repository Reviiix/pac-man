using System;
using System.Collections;
using GridSystem.GridItems;
using UnityEngine;

namespace Movement
{
    [Serializable]
    public class MovingObject
    {
        private const float NewPositionDistanceTolerance = 0.01f;
        private const int CurrentPositionDistanceTolerance = 10;
        public Transform transform;
        [SerializeField] private int speed;
        [SerializeField] protected GridItem currentPosition;
        [SerializeField] private GridItem nextPosition;
        private WaitUntil waitUntilReachedCurrentTarget;
        private bool canMove;
        private bool moving;
        private Coroutine directionChange;
        public MovementManager.Direction currentDirection;
        private MonoBehaviour coroutiner;
        
        public void Initialise(MonoBehaviour coroutineHandler)
        {
            coroutiner = coroutineHandler;
            waitUntilReachedCurrentTarget = new WaitUntil(() => !moving);
            SetNextPosition(GetNextPosition(currentDirection));
            canMove = true;
        }
        
        public void CheckPosition()
        {
            if (!canMove) return;
            if (nextPosition is Wall) return;
            moving = true;
            if (NewPositionReached())
            {
                OnDestinationReached();
            }
        }
        
        protected virtual void OnDestinationReached()
        {
            SetCurrentPosition(nextPosition);
            MoveToCurrentGridItem();
            SetNextPosition(GetNextPosition(currentDirection));
            moving = false;
        }

        private void MoveToCurrentGridItem()
        {
            transform.position = currentPosition.transform.position;
        }

        public void DetermineDirectionChange(KeyCode key)
        {
            if (directionChange != null)
            {
                coroutiner.StopCoroutine(directionChange);
            }
            
            if (CloseToCurrentGridItem())
            {
                ChangeDirection(GetDirectionFromKeyCode(key));
            }
            else
            {
                directionChange = coroutiner.StartCoroutine(WaitUntilTargetReached(()=>ChangeDirection(GetDirectionFromKeyCode(key))));
            }
        }

        protected void ChangeDirection(MovementManager.Direction direction)
        {
            if (currentPosition.GetAdjacentItem(direction) is Wall) return;
            currentDirection = direction;
            SetCurrentPosition(currentPosition);
            SetNextPosition(currentPosition.GetAdjacentItem(currentDirection));
        }
        
        public Vector3 GetDestination()
        {
            var targetDestination = nextPosition.transform.position;
            return Vector3.MoveTowards(transform.position, targetDestination, speed * Time.deltaTime);
        }

        private MovementManager.Direction GetDirectionFromKeyCode(KeyCode key)
        {
            return key switch
            {
                KeyCode.W => MovementManager.Direction.Up,
                KeyCode.A => MovementManager.Direction.Left,
                KeyCode.S => MovementManager.Direction.Down,
                KeyCode.D => MovementManager.Direction.Right,
                _ => currentDirection
            };
        }

        private bool CloseToCurrentGridItem()
        {
            return Vector3.Distance(transform.position, currentPosition.transform.position) < CurrentPositionDistanceTolerance;
        }

        private bool NewPositionReached()
        {
            return Vector3.Distance(transform.position, nextPosition.transform.position) < NewPositionDistanceTolerance;
        }

        private void SetCurrentPosition(GridItem position)
        {
            currentPosition = position;
        }
        
        private void SetNextPosition(GridItem position)
        {
            nextPosition = position;
        }

        private GridItem GetNextPosition(MovementManager.Direction direction)
        {
            var gridItem = nextPosition.GetAdjacentItem(direction);
            return gridItem is Wall ? currentPosition : gridItem;
        }
        
        private IEnumerator WaitUntilTargetReached(Action callBack)
        {
            yield return waitUntilReachedCurrentTarget;
            callBack();
        }
    }
}