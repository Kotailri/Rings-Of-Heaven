using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnRun : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
