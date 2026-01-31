using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private const float MoveSpeed = 20f;
    private Rigidbody2D _rigidbody2D;
    private Vector3 _moveDir;
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float moveX = 0f;
        float moveY = 0f;
        
        if (Input.GetKey(KeyCode.W))
        {
            moveY = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveY = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = +1f;
        }

        _moveDir = new Vector3(moveX, moveY).normalized;
    }

    private void FixedUpdate()
    {
        _rigidbody2D.velocity = _moveDir * MoveSpeed;

    }
}
