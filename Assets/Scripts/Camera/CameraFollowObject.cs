using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    public Transform _playerTransform;

    [Header("Flip Rotation Stats")]
    public float _flipYRotationTime = 0.5f;

    private PlayerMovement pm;


    private void Awake()
    {
        pm = _playerTransform.gameObject.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        transform.position = _playerTransform.position; 
    }

    public void CallTurn()
    {
        LeanTween.rotateY(gameObject, DetermineEndRotation(), _flipYRotationTime).setEaseInOutSine();
    }

    private float DetermineEndRotation()
    {
        if (pm.facing == PlayerRBFacingDirection.Right)
        {
            return 0f;
        }

        if (pm.facing == PlayerRBFacingDirection.Left)
        {
            return 180f;
        }

        return 0f;
    }
}
