using UnityEngine;

public class PlayerGrounded : MonoBehaviour
{
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2   _groundCheckSize = new(0.49f, 0.03f);
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _iceLayer;

    public bool IsGrounded { get; set; }
    public bool IsIcy { get; set; }

    private void Update()
    {
        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) || // Grounded if on ground or ice
            Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _iceLayer))
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }

        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer)) // Not icy if on ground
        {
            IsIcy = false;
        }

        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _iceLayer)) // Icy if on ice
        {
            IsIcy = true;
        }

        if (Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) < 0.1f) // Cancel ice on low velocity
        {
            IsIcy = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }
}
