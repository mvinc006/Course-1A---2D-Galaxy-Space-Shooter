using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile_Behaviour : MonoBehaviour
{
    [SerializeField] float _lifeTime;
    [SerializeField, Range(1,60)] float speed = 6;
    [SerializeField, Range(1, 300)] float _maxRotationSpeed = 300;
    [SerializeField] Transform _target;
    private Rigidbody2D rb;
    private bool isSearchingForTarget;
    private float trackingTimer = 0f;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(this.gameObject, _lifeTime);
    }

    private void FixedUpdate()
    {
        if (_target)
            MoveToTarget();
        else
        {
            rb.MovePosition(transform.position + (Vector3.up * speed * Time.deltaTime));
            FindTarget();
        }
            
    }

    private void MoveToTarget()
    {
        Vector2 pointToTarget = (Vector2)transform.position - (Vector2)_target.transform.position;
        pointToTarget.Normalize();

        float value = Vector3.Cross(pointToTarget, transform.up).z;       

        rb.angularVelocity = value * _maxRotationSpeed;
        rb.velocity = transform.up * speed;
    }

    private void FindTarget()
    {        
        if (Time.deltaTime > trackingTimer)
            trackingTimer = Time.deltaTime + 0.5f;
        else
            return;

        float closestDistance = Mathf.Infinity;
        Enemy bestTarget = null;

        // Iterate through the scenes list of enemies and check which is the closest to us.
        foreach (Enemy MissileTarget in SpawnManager.ActiveEnemies)
        {
            // check if the target is already tracked
            if (!MissileTarget.bIsMissleTracked())
            {
                Vector2 distance = MissileTarget.transform.position - transform.position;
                if (distance.sqrMagnitude < (closestDistance * closestDistance))
                {
                    bestTarget = MissileTarget;
                }
            }
        }

        if (bestTarget != null)
        {
            bestTarget.OnMissileTracked(true);            
            _target = bestTarget.transform;            
        }        
    }

    private void OnDestroy()
    {
        if (_target != null && _target.TryGetComponent(out Enemy MissileTarget))
            MissileTarget.OnMissileTracked(false);            
    }

}
