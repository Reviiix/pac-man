using System;
using System.Collections;
using GridSystem.GridItems;
using UnityEngine;
using UnityEngine.Serialization;

namespace Movement.MovingObjects
{
    [Serializable]
    public abstract class MovingObject
    {
        private const float NewPositionDistanceTolerance = 0.01f;
        private const int CurrentPositionDistanceTolerance = 10;
        public Transform transform;
        [SerializeField] protected int speed;
        [FormerlySerializedAs("currentPosition")] [SerializeField] protected GridItem current;
        [FormerlySerializedAs("nextPosition")] [SerializeField] protected GridItem next;
        private WaitUntil waitUntilReachedCurrentTarget;
        protected bool Moving;
        private Coroutine directionChange;
        [FormerlySerializedAs("currentDirection")] public MovementManager.Direction currentMovementDirection;
        private MonoBehaviour coroutiner;
        
        public void Initialise(MonoBehaviour coroutineHandler)
        {
            coroutiner = coroutineHandler;
            waitUntilReachedCurrentTarget = new WaitUntil(() => !Moving);
            SetNextPosition(GetAdjacentItem(currentMovementDirection));
        }
        
        public virtual void CheckPosition()
        {
            if (next is Wall) return;
            Moving = true;
            if (NewPositionReached())
            {
                OnDestinationReached();
            }
        }
        
        protected virtual void OnDestinationReached()
        {
            SetCurrentPosition(next);
            SnapToExactCurrentGridItem();
            SetNextPosition(GetAdjacentItem(currentMovementDirection));
            Moving = false;
        }

        private void SnapToExactCurrentGridItem()
        {
            transform.position = current.transform.position;
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
            if (current.GetAdjacentItem(direction) is Wall) return;
            currentMovementDirection = direction;
            SetCurrentPosition(current);
            SetNextPosition(current.GetAdjacentItem(currentMovementDirection));
        }
        
        public Vector3 GetDestination()
        {
            return Vector3.MoveTowards(transform.position, next.transform.position, speed * Time.deltaTime);
        }

        private MovementManager.Direction GetDirectionFromKeyCode(KeyCode key)
        {
            return key switch
            {
                KeyCode.W => MovementManager.Direction.Up,
                KeyCode.A => MovementManager.Direction.Left,
                KeyCode.S => MovementManager.Direction.Down,
                KeyCode.D => MovementManager.Direction.Right,
                _ => currentMovementDirection
            };
        }

        private bool CloseToCurrentGridItem()
        {
            return Vector3.Distance(transform.position, current.transform.position) <= CurrentPositionDistanceTolerance;
        }

        protected virtual bool NewPositionReached()
        {
            return Vector3.Distance(transform.position, next.transform.position) <= NewPositionDistanceTolerance;
        }

        private void SetCurrentPosition(GridItem position)
        {
            current = position;
        }
        
        private void SetNextPosition(GridItem position)
        {
            next = position;
        }

        private GridItem GetAdjacentItem(MovementManager.Direction direction)
        {
            var gridItem = next.GetAdjacentItem(direction);
            return gridItem is Wall ? current : gridItem;
        }
        
        private IEnumerator WaitUntilTargetReached(Action callBack)
        {
            yield return waitUntilReachedCurrentTarget;
            callBack();
        }

        public GridItem GetNextPosition()
        {
            return next;
        }
    }
}