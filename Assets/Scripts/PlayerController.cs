using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Movement
    [Header("Movement")]
    private Rigidbody2D rb2D;

    private float movement = 0f;

    [SerializeField] private float movementSpeed;
    [Range(0, 0.3f)] [SerializeField] private float movementSmoothing;

    private Vector3 speed = Vector3.zero;
    private bool rightDirection = true;

    //Jump
    [Header["Jump"]]
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask isFloor;
    [SerializeField] private Transform floorController;
    [SerializeField] private Vector3 boxDimensions;
    [SerializeField] private bool onTheFloor;

    private bool jump = false;

    //Start
    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        movement = Input.GetAxisRaw("Horizontal") * movementSpeed;
        if(Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    private void FixedUpdate()
    {
        onTheFloor = Physics2D.OverlapBox(floorController.position, boxDimensions, 0f, isFloor);
        Move(movement * Time.fixedDeltaTime, jump);

        jump = false;
    }

    private void Move(float move, bool jump)
    {
        Vector3 targetSpeed = new Vector2(move, rb2D.velocity.y);
        rb2D.velocity = Vector3.SmoothDamp(rb2D.velocity, targetSpeed, ref speed, movementSmoothing);

        if (move>0 && !rightDirection)
        {
            TurnLeftOrRight();
        }
        else if (move<0 && rightDirection)
        {
            TurnLeftOrRight();
        }

        if (onTheFloor && jump)
        {
            onTheFloor = false;
            rb2D.AddForce(new Vector2(0f, jumpForce));
        }
    }

    private void TurnLeftOrRight()
    {
        rightDirection = !rightDirection;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(floorController.position, boxDimensions);
    }
}
