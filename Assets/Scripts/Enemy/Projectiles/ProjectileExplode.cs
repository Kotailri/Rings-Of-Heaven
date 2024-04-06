using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileExplode : MonoBehaviour
{
    public GameObject Explosion;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || Utility.IsOfTag(collision.gameObject, Tags.BouncesRing) || collision.gameObject.CompareTag("Ring"))
        {
            Instantiate(Explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
