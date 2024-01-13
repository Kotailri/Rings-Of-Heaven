using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    private Transform _playerTransform;

    [Header("Flip Rotation Stats")]
    public float _flipYRotationTime = 0.5f;

    private PlayerFacing _playerFacing;


    private void Awake()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
        _playerFacing =  _playerTransform.gameObject.GetComponent<PlayerFacing>();
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
        if (_playerFacing.FacingDirection == OrthogonalDirection.Right)
        {
            return 0f;
        }

        if (_playerFacing.FacingDirection == OrthogonalDirection.Left)
        {
            return 180f;
        }

        return 0f;
    }
}
