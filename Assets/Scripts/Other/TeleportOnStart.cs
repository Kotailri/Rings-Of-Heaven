using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportOnStart : MonoBehaviour
{
    public GameObject objectToMove;
    public GameObject objectToMoveTo;

    // Start is called before the first frame update
    void Start()
    {
        objectToMove.transform.position = objectToMoveTo.transform.position;   
    }
}
