using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    bool canJump;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("left") || Input.GetKey("a"))
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-750f * Time.deltaTime, 0));
            gameObject.GetComponent<Animator>().SetBool("moving", true);
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }

        if (Input.GetKey("right") || Input.GetKey("d"))
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(750f * Time.deltaTime, 0));
            gameObject.GetComponent<Animator>().SetBool("moving", true);
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }

        if (!Input.GetKey("left") && !Input.GetKey("a") && !Input.GetKey("right") && !Input.GetKey("d"))
        {
            gameObject.GetComponent<Animator>().SetBool("moving", false);
        }

        if (Input.GetKeyDown("up") && canJump || Input.GetKeyDown("w") && canJump)
        {
            canJump = false;
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 250f));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            canJump = true;
        }
    }
}
