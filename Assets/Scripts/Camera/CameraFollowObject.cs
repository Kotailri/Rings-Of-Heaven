using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    public Transform _playerTransform;

    [Header("Flip Rotation Stats")]
    public float _flipYRotationTime = 0.5f;

    private PlayerFacing playerFacing;


    private void Awake()
    {
        playerFacing = _playerTransform.gameObject.GetComponent<PlayerFacing>();
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
        if (playerFacing.FacingDirection == OrthogonalDirection.Right)
        {
            return 0f;
        }

        if (playerFacing.FacingDirection == OrthogonalDirection.Left)
        {
            return 180f;
        }

        return 0f;
    }
}
