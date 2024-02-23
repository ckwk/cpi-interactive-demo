using System;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [SerializeField]
    float waitTime = 5,
        additionalStartWaitTime = 0,
        waitTimer,
        startOffset,
        moveSpeed = 4;

    [SerializeField]
    int startingMovementPointIndex = 0;

    [SerializeField]
    List<Transform> movementPoints;

    Transform movementPointsObject,
        currentMovementPoint;

    int currentMovementPointIndex;
    List<int> movementPointsToWaitOn = new List<int>() { 0, 1, 4, 7, 8, 11 };

    public bool justActivated = true;

    void Start()
    {
        waitTimer = additionalStartWaitTime;
        movementPoints = new List<Transform>();
        movementPointsObject = transform.parent.parent.GetChild(0);
        currentMovementPointIndex = startingMovementPointIndex;
        foreach (Transform movementPoint in movementPointsObject)
        {
            movementPoints.Add(movementPoint);
        }
        currentMovementPoint = movementPoints[currentMovementPointIndex];
        startOffset = Vector3.Distance(transform.position, currentMovementPoint.position);
    }

    void Update()
    {
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        if (Physics.OverlapSphere(transform.position, 0.8f).Length > 10 && justActivated)
        {
            return;
        }
        justActivated = false;

        var reachedWaitPoint = movementPointsToWaitOn.Contains(currentMovementPointIndex);

        if (
            Vector3.Distance(transform.position, currentMovementPoint.position)
            < (0.1 + startOffset * Convert.ToInt16(reachedWaitPoint))
        )
        {
            waitTimer = reachedWaitPoint ? waitTime : 0;
            currentMovementPointIndex =
                currentMovementPointIndex == movementPoints.Count - 1
                    ? 0
                    : currentMovementPointIndex + 1;
            currentMovementPoint = movementPoints[currentMovementPointIndex];
        }
        transform.LookAt(currentMovementPoint);
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed, Space.Self);
    }

    void OnEnable()
    {
        justActivated = !justActivated;
    }
}
