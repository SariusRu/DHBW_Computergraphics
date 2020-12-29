using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class FirstPersonControls : MonoBehaviour {

    public Camera ThirdPerson;
    public Camera StartCamera;

    public float movementSpeed = 7.5f;
    public float jumpSpeed = 7.5f;

    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    // Use this for initialization
    void Start () {

        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
	
	// Update is called once per frame
	void Update () {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        float curSpeedX = canMove ? ( movementSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? ( movementSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetKey(KeyCode.Space) && canMove)
        {
            moveDirection.y = jumpSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && canMove)
        {
            moveDirection.y = -jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }        

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
        moveDirection.y = movementDirectionY;

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            ThirdPerson.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);            
        }
    }

}
