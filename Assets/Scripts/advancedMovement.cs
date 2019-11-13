using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleInputNamespace;

public class advancedMovement : MonoBehaviour {

    public float runSpeed = 5;
    public float turnSpeed = 10;
    public float jumpForce = 2;

    Vector2 input;
    float angle;

    public Transform mainCamera;

    bool isGrounded = true;

    float axis;

    
    void FixedUpdate() {
        // Continuously check if player is trying to move in a direction
        GetInput();
        
        // Don't move if player is not trying to move
        if ((Mathf.Abs(input.x ) < 0.01f) && (Mathf.Abs(input.y) < 0.01f)) return;

        CalculateDirection();
        Rotate();
        Move();
    }

    void Update() {
        if (((SimpleInput.GetAxis("Jump") >= 0.01f) || (Input.GetButtonDown("Jump"))) && (isGrounded)) {
            // Jump if the jump key is pressed and if the player is on the ground
            Jump();
        }
    }

    void GetInput() {
        // Get horizontal and vertical input
        input.x = SimpleInput.GetAxisRaw("Horizontal");
        input.y = SimpleInput.GetAxisRaw("Vertical");
        
        // Based on the input, play the appropriate animation in the animator (idle, walk or run)
        // In the Animator, there's a particular animation to be played in the range 0 - 1, that's why
        // we need to get the bigger of the 2 inpux axes and and convert it into a positive value.
        float speed = Mathf.Max(Mathf.Abs(input.x), Mathf.Abs(input.y));
        GetComponent<Animator>().SetFloat("Speed", speed);
    }

    void CalculateDirection() {
        // Calculate angle the player needs to turn to go in the desired direction
        angle = Mathf.Atan2(input.x, input.y);
        angle = Mathf.Rad2Deg * angle;
        angle += mainCamera.eulerAngles.y;
    }

    void Rotate() {
        // Rotate the player smoothly to go in the chosen direction
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    void Move() {
        // Based on a defined value and input, move player forward
        transform.position += transform.forward * runSpeed * Time.deltaTime * Mathf.Max(Mathf.Abs(input.x), Mathf.Abs(input.y));
    }

    void Jump() {
        // Stop playing all animations and apply a vertical force to the player
        isGrounded = false;
        GetComponent<Animator>().speed = 0.0f;
        GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);   
    }

    void OnCollisionStay(Collision col) {
        // Tell the script that the player is on the ground so that he can jump again
        if (col.gameObject.CompareTag("Ground")) {
            isGrounded = true;
        }
        GetComponent<Animator>().speed = 1.0f;
    }
}
