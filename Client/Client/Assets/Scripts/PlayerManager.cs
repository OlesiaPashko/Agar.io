using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;
    public float mass = 100;
    public int id = 0;
    public static PlayerManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }
    /*void FixedUpdate()
    {
        if (isMoving)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - transform.position).normalized;
            //var velocity = new Vector2(direction.x * speed, direction.y * speed);
            //transform.position += new Vector3(0.1f * velocity.x, 0.1f * velocity.y, 0);
            ConnectionManager.instance.SendDirection(direction);
            Vector2 newPosition = ConnectionManager.instance.ReceivePosition();
            transform.position = new Vector3(newPosition.x, newPosition.y, 0);
        }
    }
    */

    public void Grow(int mass)
    {
        Debug.Log("Grow");
        this.speed -= mass*0.05f;
        Debug.Log("Mass = " + mass);
        Debug.Log("This.mass = " + this.mass);
        Debug.Log("mass/this.mass= " + mass / this.mass);
        float scale = transform.localScale.x + mass/this.mass;
        this.mass += mass;
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
