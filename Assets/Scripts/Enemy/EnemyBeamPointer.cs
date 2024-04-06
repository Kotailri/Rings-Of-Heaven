using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeamPointer : MonoBehaviour
{
    public GameObject BeamObject;
    public Transform BeamStartLocation;


    private Transform _playerTransform;

    private void Start()
    {
        _playerTransform = Utility.GetPlayer().transform;
    }

    public void SpawnBeam()
    {
        Vector2 direction = _playerTransform.position - BeamStartLocation.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject _beam = Instantiate(BeamObject, BeamStartLocation.position, Quaternion.identity);
        _beam.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
