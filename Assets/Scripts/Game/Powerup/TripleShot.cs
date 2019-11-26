using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleShot : Powerup_Base
{

    [SerializeField] float _duration = 5f;
    [SerializeField] float _fireSpeed = 1.0f;    
    private IEnumerator Timer;
    private bool _canFire = false;
    private float _fireCheck = -1f;
    private Transform _parent;

    public void OnFireTripleShot()
    {
        Instantiate(_prefab, _parent.position, Quaternion.identity).name = "TripleShot";
      //  AudioSource.PlayClipAtPoint(_manager.GetLaserSound, transform.position, 0.5f);
    }

    public void OnTripleShotActive(WaitForSeconds duration)
    {
        _canFire = true;
        InvokeWeaponChanged(this, true);

        if (Timer != null)
            StopCoroutine(Timer);        

        Timer = TripleShotTimer(duration);
        StartCoroutine(Timer);
    }

    private IEnumerator TripleShotTimer(WaitForSeconds duration)
    {
        yield return duration;
        _canFire = false;
        InvokeWeaponChanged(this, false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            OnPickup(this, true);
            OnTripleShotActive(new WaitForSeconds(_duration));
            _parent = collision.transform;
        }            
    }

    protected override void OnDisableOtherWeapons(object sender)
    {
        base.OnDisableOtherWeapons(sender);
        Destroy(this.gameObject);
    }
    protected override void Update()
    {
        base.Update();

        if (Input.GetKey(KeyCode.Space) && _canFire == true && Time.time >=_fireCheck)
        {
            _fireCheck = Time.time + _fireSpeed;
            OnFireTripleShot();
        }
    }

}
