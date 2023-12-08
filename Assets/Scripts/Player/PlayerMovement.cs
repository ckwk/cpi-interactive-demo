using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // movement vars
    public float moveSpeed = 10;
    float vSpeed = 0;
    float hSpeed = 0;

    [SerializeField]
    float friction = 0.1f,
        xMin,
        xMax,
        yMin,
        yMax,
        zMin,
        zMax;
    KeyCode left = KeyCode.LeftArrow,
        right = KeyCode.RightArrow,
        forward = KeyCode.UpArrow,
        backward = KeyCode.DownArrow;

    // camera vars
    [SerializeField]
    Transform cameraTrans;
    public float lookSpeed = 3;
    Vector2 rotation;

    void Update()
    {
        var dt = Time.deltaTime; // store deltaTime in a local var so we don't have to waste resources constantly getting it
        var dtFriction = friction * dt;

        // camera arrow key movement
        var vDir = BoolToInt(Input.GetKey(forward)) - BoolToInt(Input.GetKey(backward)); // gets a -1, 0, 1 value, 1=forward, -1=backward
        var hDir = BoolToInt(Input.GetKey(right)) - BoolToInt(Input.GetKey(left)); // gets a -1, 0, 1 value, 1=right, -1=left
        var adjustedMoveSpeed = moveSpeed; //* (transform.position.y / 15);

        vSpeed += vDir != 0 ? dtFriction * vDir : dtFriction * -Math.Sign(vSpeed); // smoothly increases/decreases forward/backward speed when holding a movement key
        vSpeed = Math.Clamp(vSpeed, -adjustedMoveSpeed, adjustedMoveSpeed); // keeps speed within moveSpeed boundaries
        var forwardVec = dt * vSpeed * Vector3.forward;

        hSpeed += hDir != 0 ? dtFriction * hDir : dtFriction * -Math.Sign(hSpeed); // same as above, but for l/r movement
        hSpeed = Math.Clamp(hSpeed, -adjustedMoveSpeed, adjustedMoveSpeed);
        var rightVec = dt * hSpeed * Vector3.right;

        transform.Translate(forwardVec, Space.Self);
        transform.Translate(rightVec, Space.Self);
        transform.Translate(Vector3.up * -Input.mouseScrollDelta); // scroll movement for height

        KeepWithinBoundaries(transform.position, dt, xMin, xMax, yMin, yMax, zMin, zMax);

        // mouselook
        rotation += new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        var hRotation = new Vector2(0, rotation.y);
        cameraTrans.eulerAngles = rotation * lookSpeed; // rotates the camera in accordance with mouse movement
        transform.eulerAngles = hRotation * lookSpeed; // rotates the player object in accordance with horizontal mouse movement so that the collision box's angle isn't changing to help with collision
    }

    int BoolToInt(bool toConvert) => toConvert == true ? 1 : 0;

    // keeps the pos Vector within the constraints on a global coordinate scale
    // dt=Time.deltaTime, passed for optimization purposes
    void KeepWithinBoundaries(
        Vector3 pos,
        float dt,
        float xMin,
        float xMax,
        float yMin,
        float yMax,
        float zMin,
        float zMax
    )
    {
        // sets the x component of the vector to within the xMin - xMax range
        var xCoord = pos.x > xMin ? pos.x : xMin + dt;
        xCoord = pos.x < xMax ? xCoord : xMax - dt;

        // sets the y component of the vector to within the yMin - yMax range
        var yCoord = pos.y > yMin ? pos.y : yMin + dt;
        yCoord = pos.y < yMax ? yCoord : yMax - dt;

        // sets the z component of the vector to within the zMin - zMax range
        var zCoord = pos.z > zMin ? pos.z : zMin + dt;
        zCoord = pos.z < zMax ? zCoord : zMax - dt;

        transform.position = new Vector3(xCoord, yCoord, zCoord);
    }
}
