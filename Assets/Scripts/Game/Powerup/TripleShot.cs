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

    private void OnFireTripleShot()
    {
        Instantiate(_prefab, _parent.position, Quaternion.identity).name = "TripleShot";
      //  AudioSource.PlayClipAtPoint(_manager.GetLaserSound, transform.position, 0.5f);
    }

    private void OnPowerupEnabled(WaitForSeconds duration)
    {
        _canFire = true;
        OnWeaponChange(this, true);

        if (Timer != null)
            StopCoroutine(Timer);        

        Timer = PowerupTimer(duration);
        StartCoroutine(Timer);
    }

    private IEnumerator PowerupTimer(WaitForSeconds duration)
    {
        yield return duration;
        _canFire = false;
        OnWeaponChange(this, false);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            OnPowerupPickedUp(this, true);
            OnPowerupEnabled(new WaitForSeconds(_duration));
            _parent = collision.transform;
        }            
    }

    protected override void OnPowerupChangedEvent(object sender, bool removeFromListener)
    {
        base.OnPowerupChangedEvent(sender, true);                
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
