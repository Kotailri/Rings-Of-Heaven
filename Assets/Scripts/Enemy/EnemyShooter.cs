using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public GameObject Projectile;
    public Transform ShootPosition;

    [Space(5f)]
    public int NumberOfShots;
    public float TimeBetweenShots;

    [Space(5f)]
    public float ShootChance;
    public float ShootTimer;

    private float _currentShootTime = 0f;

    private void Shoot()
    {
        GetComponent<EnemyBeamPointer>().SpawnBeam();

        if (ShootPosition == null)
        {
            Instantiate(Projectile, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(Projectile, ShootPosition.position, Quaternion.identity);
        }
    }

    private IEnumerator ShootCoroutine()
    {
        GetComponent<EnemyFacing>().PauseEnemyFacing(TimeBetweenShots * NumberOfShots);

        Shoot();
        for (int i = 1; i < NumberOfShots; i++)
        {
            yield return new WaitForSeconds(TimeBetweenShots);
            Shoot();
        }
    }

    private void Update()
    {
        _currentShootTime += Time.deltaTime;

        if (_currentShootTime >= ShootTimer)
        {
            if (Random.Range(0f,1f) <= ShootChance)
            {
                if (NumberOfShots == 1)
                {
                    Shoot();
                }
                else
                {
                    StartCoroutine(ShootCoroutine());
                }
                
            }
            
            _currentShootTime = 0f;
        }

    }

}
