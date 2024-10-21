using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : GameUnit
{
    public Rigidbody rb;

    public override void OnInit()
    {
        rb.velocity = Vector3.forward * 5f;
    }

    public override void OnDespawn()
    {
        SimplePool.Despawn(this);
    }


    private void OnTriggerEnter(Collider other)
    {
        OnDespawn();
    }
}
