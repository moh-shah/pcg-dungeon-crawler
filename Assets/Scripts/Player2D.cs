using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2D : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    private Vector2 _velocity;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _velocity = new Vector2(Input.GetAxis("Horizontal"),  Input.GetAxis("Vertical")).normalized*10;
        
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position+_velocity*Time.fixedDeltaTime);
    }
}