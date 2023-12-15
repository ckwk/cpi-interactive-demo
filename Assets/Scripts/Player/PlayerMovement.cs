using UnityEngine.InputSystem;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    // movement vars
    Controls controls;
    InputAction move;

    [SerializeField]
    bool is3D;

    [SerializeField]
    float lookSpeed = 3,
        moveSpeed = 10,
        xMin,
        xMax,
        yMin,
        yMax,
        zMin,
        zMax;

    // camera vars
    [SerializeField]
    GameObject cameraObject;
    Transform cameraTrans;
    Camera cameraCam;
    Vector2 rotation;

    // Awake, OnEnable, and OnDisable are required by the InputSystem library
    void Awake()
    {
        controls = new Controls();
        cameraTrans = cameraObject.GetComponent<Transform>();
        cameraCam = cameraObject.GetComponent<Camera>();
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
        var dt = Time.deltaTime; // store deltaTime for optimization
        var intIs3D = BoolToInt(is3D); // store is3D in an int for optimization

        // movement
        var moveVec = move.ReadValue<Vector2>();
        var moveDir = is3D ? new Vector3(moveVec.x, 0, moveVec.y) : (Vector3)moveVec; // swap between 2d or 3d coordinates depending on is3D
        var scrollDelta = -Input.mouseScrollDelta.y;
        var cameraSize = cameraCam.orthographicSize;

        transform.Translate(moveDir * dt * moveSpeed, Space.Self); // WASD for horizontal movement
        transform.Translate(Vector3.up * intIs3D * scrollDelta); // scroll for veritcal movement in 3D

        // scroll for zoom in 2D
        cameraCam.orthographicSize = Math.Clamp(
            cameraSize + scrollDelta * (1 - intIs3D),
            zMin,
            zMax
        );

        // clamp location within the boundaries of the map
        var pos = transform.position;
        transform.position = new Vector3(
            Math.Clamp(pos.x, xMin, xMax),
            Math.Clamp(pos.y, yMin, yMax),
            Math.Clamp(pos.z, zMin, zMax) - (100 * (1 - intIs3D)) // adjust clamping for zoom instead of position in 2D
        );

        // mouselook
        rotation += new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * intIs3D; // adjusts rotation based on mouse movement when in 3D
        var camRotation = rotation * lookSpeed;

        cameraTrans.eulerAngles = new Vector2(Math.Clamp(camRotation.x, 0, 80), camRotation.y); // clamp camera rotation to not be crazy
        transform.eulerAngles = new Vector2(0, rotation.y) * lookSpeed; // rotates the player object in accordance with horizontal mouse movement so that the collision box's angle isn't changing to help with collision
    }

    int BoolToInt(bool toConvert) => toConvert == true ? 1 : 0;
}
