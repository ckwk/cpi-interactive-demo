using UnityEngine.InputSystem;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    // movement vars
    [SerializeField]
    bool is3D;

    [SerializeField]
    float lookSpeed = 3,
        moveSpeed = 10,
        friction = 0.1f,
        xMin,
        xMax,
        yMin,
        yMax,
        zMin,
        zMax;

    Controls controls;
    InputAction move;

    // camera vars
    [SerializeField]
    Transform cameraTrans;
    Vector2 rotation;

    // Awake, OnEnable, and OnDisable are required by the InputSystem library
    void Awake()
    {
        controls = new Controls();
    }

    void OnEnable()
    {
        controls.Enable();
        move = controls.Player.Move;
        move.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
        move.Disable();
    }

    void Update()
    {
        var dt = Time.deltaTime; // store deltaTime in a local var so we don't have to waste resources constantly getting it
        var moveVec = move.ReadValue<Vector2>();
        var moveDir = is3D ? new Vector3(moveVec.x, 0, moveVec.y) : (Vector3)moveVec; // swap between 2d or 3d coordinates depending on is3D

        transform.Translate(moveDir * dt * moveSpeed, Space.Self); // WASD movement
        transform.Translate(Vector3.up * -Input.mouseScrollDelta); // scroll movement for height
        KeepWithinBoundaries(transform.position, dt, xMin, xMax, yMin, yMax, zMin, zMax);

        // mouselook
        rotation +=
            new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * BoolToInt(is3D); // adjusts rotation based on mouse movement when in 3D
        var hRotation = new Vector2(0, rotation.y);
        var camRotation = rotation * lookSpeed;
        cameraTrans.eulerAngles = new Vector3(Math.Clamp(camRotation.x, 0, 80), camRotation.y); // clamp camera rotation to not be crazy
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
