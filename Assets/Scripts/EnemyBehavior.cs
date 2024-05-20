using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform floorController;
    [SerializeField] private float distance;
    [SerializeField] private bool rightDirection;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        RaycastHit2D floorInformation = Physics2D.Raycast(floorController.position, Vector2.down, distance);

        rb.velocity = new Vector2(speed, rb.velocity.y);

        if (floorInformation == false)
        {
            TurnDirection();
        }
    }

    private void TurnDirection()
    {
        rightDirection = !rightDirection;
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 100, 0);
        speed *= -1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(floorController.transform.position, floorController.transform.position + Vector3.down * distance);
    }
}
