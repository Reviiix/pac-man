using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Movement
{
    /// <summary>
    /// The Job system requires a lot of boiler plate code but is the most optimised development strategy in Unity 202x.
    /// </summary>
    public static class PathfindingJobs
    {
        public static void MoveObjects(Transform[] objectsToMove, Vector3[] positions)
        {
            //Create primitive/native variables to pass to job.
            var amountOfObjects = objectsToMove.Length;
            var objectPositions = new NativeArray<float3>(amountOfObjects, Allocator.TempJob);
            var endPositions = new NativeArray<float3>(amountOfObjects, Allocator.TempJob);
            
            //Compose variables.
            for (var i = 0; i < amountOfObjects; i++)
            {
                objectPositions[i] = new float3(objectsToMove[i].position);
                endPositions[i]= new float3(positions[i]);
            }
            
            //Create job handle and pass in values.
            var jobHandle = new MoveMultipleObjectsTowardsPosition
            {
                PositionsOfObjectsToMove = objectPositions,
                PositionsToMoveTo = endPositions,
            };

            //Run the job with the handle.
            var handle = jobHandle.Schedule(amountOfObjects, 10);
            handle.Complete();
    
            //Reassign the position values (Having had the job applied).
            for (var i = 0; i < amountOfObjects; i++)
            {
                objectsToMove[i].transform.position = objectPositions[i];
            }
    
            //Dispose of native arrays (Not handled by garbage collector).
            objectPositions.Dispose();
            endPositions.Dispose();
        }

        [BurstCompile]
        private struct MoveMultipleObjectsTowardsPosition : IJobParallelFor
        {
            public NativeArray<float3> PositionsOfObjectsToMove;
            public NativeArray<float3> PositionsToMoveTo;

            public void Execute(int index)
            {
                PositionsOfObjectsToMove[index] = PositionsToMoveTo[index];
            }
        }
  
    }
}
