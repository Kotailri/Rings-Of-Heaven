using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    private void Awake()
    {
        if (!Config.HitboxDebugMode)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
