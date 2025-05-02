using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    public PlayerinputControl inputControl;
    public Vector2 inputDirection;
    public float speed;
    private Rigidbody2D rb;
    private void Awake()
    {
        inputControl = new PlayerinputControl();
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        inputControl.Enable();
    }
    private void OnDisable()
    {
        inputControl.Disable();
    }
    private void Update()
    {
        inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();
    }
    private void FixedUpdate()
    {
        Move();
    }
    public void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime,inputDirection.y* speed*Time.deltaTime);
        int faceDir = (int)transform.localScale.x;
        if(inputDirection.x > 0 ) 
            faceDir = 1;
        else if(inputDirection.x < 0 )
            faceDir = -1; 
        transform.localScale = new Vector3(faceDir, 1, 1);
    }
}
