using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayerOnHit : MonoBehaviour
{
    public int ContactDamage;
    public bool DestroyOnContact;

    public int GetContactDamage()
    {
        if (DestroyOnContact)
            Destroy(gameObject);
        return ContactDamage;
    }
}
