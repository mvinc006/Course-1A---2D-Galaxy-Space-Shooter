using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile_Behaviour : MonoBehaviour
{    
    [SerializeField, Range(1,60)] float speed = 6;
    [SerializeField, Range(1, 300)] float _maxRotationSpeed = 300;
    [SerializeField] Transform _target;
    private Rigidbody2D rb;   
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if(_target)
            MoveToTarget();        
    }

    private void MoveToTarget()
    {
        Vector2 pointToTarget = (Vector2)transform.position - (Vector2)_target.transform.position;
        pointToTarget.Normalize();

        float value = Vector3.Cross(pointToTarget, transform.up).z;       

        rb.angularVelocity = value * _maxRotationSpeed;
        rb.velocity = transform.up * speed;
    }   

    public void OnSetTarget(Transform target)
    {
        _target = target;
    }


}
