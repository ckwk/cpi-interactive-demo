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
        var dt = Time.deltaTime;

        // camera arrow key movement
        var vDir = BoolToInt(Input.GetKey(forward)) - BoolToInt(Input.GetKey(backward));
        var hDir = BoolToInt(Input.GetKey(right)) - BoolToInt(Input.GetKey(left));
        var adjustedMoveSpeed = moveSpeed; //* (transform.position.y / 15);

        vSpeed += vDir != 0 ? friction * vDir : friction * -Math.Sign(vSpeed);
        vSpeed = Math.Clamp(vSpeed, -adjustedMoveSpeed, adjustedMoveSpeed);
        var forwardVec = dt * vSpeed * Vector3.forward;

        hSpeed += hDir != 0 ? friction * hDir : friction * -Math.Sign(hSpeed);
        hSpeed = Math.Clamp(hSpeed, -adjustedMoveSpeed, adjustedMoveSpeed);
        var rightVec = dt * hSpeed * Vector3.right;

        transform.Translate(forwardVec, Space.Self);
        transform.Translate(rightVec, Space.Self);
        // scroll movement for height
        transform.Translate(Vector3.up * -Input.mouseScrollDelta);

        KeepWithinBoundaries(transform.position, dt, xMin, xMax, yMin, yMax, zMin, zMax);

        // mouselook
        rotation += new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        var hRotation = new Vector2(0, rotation.y);
        cameraTrans.eulerAngles = (Vector2)rotation * lookSpeed;
        transform.eulerAngles = (Vector2)hRotation * lookSpeed;
    }

    int BoolToInt(bool toConvert) => toConvert == true ? 1 : 0;

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
        var xCoord = pos.x > xMin ? pos.x : xMin + dt;
        xCoord = pos.x < xMax ? xCoord : xMax - dt;
        var yCoord = pos.y > yMin ? pos.y : yMin + dt;
        yCoord = pos.y < yMax ? yCoord : yMax - dt;
        var zCoord = pos.z > zMin ? pos.z : zMin + dt;
        zCoord = pos.z < zMax ? zCoord : zMax - dt;

        transform.position = new Vector3(xCoord, yCoord, zCoord);
    }
}
