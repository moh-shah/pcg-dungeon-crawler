using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimStates
{
    Idle,Run
}
public class Player : MonoBehaviour
{
    public Animator animator;
    
    private Rigidbody _rigidbody;

    private Vector3 _velocity;


    private Camera cam;

    private AnimStates _animState;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        cam = Camera.main;
   
    }
    
    // Update is called once per frame
    void Update()
    {
        _velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * 10;

        if (Math.Abs(_velocity.magnitude) < .001f)
        {
            if (_animState != AnimStates.Idle)
            {
                animator.SetTrigger("Idle");
                _animState = AnimStates.Idle;
            }
        }
        else 
        {
            if (_animState != AnimStates.Run)
            {
                animator.SetTrigger("Run");
                _animState = AnimStates.Run;
            }
        }    

        var camRay = cam.ScreenPointToRay(Input.mousePosition);
        var groundPlane=new Plane(Vector3.up,Vector3.zero);
        float rayLength;
        if (groundPlane.Raycast(camRay, out rayLength))
        {
            var ptl = camRay.GetPoint(rayLength);
            Debug.DrawLine(camRay.origin,ptl,Color.red);
            
            transform.LookAt(new Vector3(ptl.x,transform.position.y,ptl.z));
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + _velocity * Time.fixedDeltaTime);
    }
}