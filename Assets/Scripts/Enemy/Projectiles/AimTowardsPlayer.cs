using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTowardsPlayer : MonoBehaviour
{
    public float ProjectileSpeed;
    private Vector3 _destination;

    private void OnEnable()
    {
        _destination = Utility.GetPlayer().transform.position - transform.position;   
    }

    private void Update()
    {
        transform.position += _destination.normalized * (Time.deltaTime * ProjectileSpeed);
    }
}
