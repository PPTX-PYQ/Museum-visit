using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController controller;
    private Transform cameraTransform;
    private Animation animation1;
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
        animation1 = GetComponent<Animation>();
    } 

    
    void Update()
    {
        HandleMovement();
    }
    
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();
        
        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();
        
        Vector3 move = cameraForward * vertical + cameraRight * horizontal;
        controller.SimpleMove(move * moveSpeed);
        
        if (move.magnitude > 0.1f)
        {
            animation1.Play("walk");
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animation1.Play("wait");
        }
    }
}
