using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrounded : MonoBehaviour
{
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new(0.49f, 0.03f);
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _iceLayer;

    public bool isGrounded;
    public bool isIcy;

    private void Update()
    {
        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) 
            || Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _iceLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
        {
            isIcy = false;
        }

        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _iceLayer))
        {
            isIcy = true;
        }

        if (Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) < 0.1f)
        {
            isIcy = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }
}
