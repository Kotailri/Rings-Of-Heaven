using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public delegate bool ThrowInputFunction();

public class Throwable
{
    public GameObject ThrowablePrefab    { get; set; }
    public GameObject ThrowableReference { get; set; }

    public bool CanThrow()
    {
        return ThrowableReference == null;
    }

    public Throwable(GameObject _throwable) 
    {
        ThrowablePrefab = _throwable;
    }
}

public class ThrowRing : MonoBehaviour
{
    [Header("Throwables")]
    public List<GameObject> ThrowableObjects = new();

    [Header("Throw Blockers")]
    public Transform ringBlockFront;
    public Transform ringBlockUp;
    public Transform ringBlockDown;

    [Space(5f)]
    public Vector2   ringBlockedSize = new(0.49f, 0.03f);
    public LayerMask ringBlockLayer;

    [Space(10f)]
    public float ringThrowCooldown;

    private bool            _canThrow = true;
    private PlayerFacing    _playerFacing;
    private List<Throwable> _throwableList = new();

    private void Awake()
    {
        _playerFacing = GetComponent<PlayerFacing>();
        foreach (GameObject obj in ThrowableObjects)
        {
            _throwableList.Add(new Throwable(obj));
        }
    }

    public void SwapThrowableOrderRight(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
            return;

        // Ensure all throwables are not out
        foreach (Throwable throwable in _throwableList)
        {
            if (!throwable.CanThrow())
            {
                return;
            }
        }

        // Shift all throwables, put index 0 at the end
        Throwable firstThrowable = _throwableList[0];
        _throwableList.RemoveAt(0);
        _throwableList.Add(firstThrowable);

        AudioManager.instance.PlaySound("click");
    }

    public void SwapThrowableOrderLeft(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
            return;

        // Ensure all throwables are not out
        foreach (Throwable throwable in _throwableList)
        {
            if (!throwable.CanThrow())
            {
                return;
            }
        }

        // Shift all throwables, end index at index 0
        Throwable firstThrowable = _throwableList[_throwableList.Count-1];
        _throwableList.RemoveAt(_throwableList.Count-1);
        _throwableList.Insert(0, firstThrowable);

        AudioManager.instance.PlaySound("click");
    }

    public void ThrowObject(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
            return;

        foreach (Throwable throwable in _throwableList)
        {
            if (throwable.CanThrow())
            {
                Throw(throwable);
                return;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(ringBlockFront.position, ringBlockedSize);
        Gizmos.DrawWireCube(ringBlockUp.position, ringBlockedSize);
        Gizmos.DrawWireCube(ringBlockDown.position, ringBlockedSize);
    }

    private void Throw(Throwable tr)
    {
        if (_canThrow)
        {
            _canThrow = false;
            Utility.InvokeLambda(() => { _canThrow = true; }, ringThrowCooldown);
        }
        else
        {
            return;
        }

        Vector2 throwdir = Vector2.zero;
        Vector3 ringAngle = Vector3.zero;
        float rangeHorizontal = 1.0f;
        float rangeVertical = 2.0f;

        switch (_playerFacing.PointingDirection)
        {
            case OrthogonalDirection.Left:
                if (Physics2D.OverlapBox(ringBlockFront.position, ringBlockedSize, 0, ringBlockLayer))
                    return;

                throwdir += new Vector2(-rangeHorizontal, 0);
                break;

            case OrthogonalDirection.Right:
                if (Physics2D.OverlapBox(ringBlockFront.position, ringBlockedSize, 0, ringBlockLayer))
                    return;

                throwdir += new Vector2(rangeHorizontal, 0);
                break;

            case OrthogonalDirection.Up:
                if (Physics2D.OverlapBox(ringBlockUp.position, ringBlockedSize, 0, ringBlockLayer))
                    return;

                ringAngle = new Vector3(transform.rotation.x, transform.rotation.y, 90);
                throwdir += new Vector2(rangeHorizontal * Mathf.Sign(transform.rotation.y) * 2, rangeVertical);
                break;

            case OrthogonalDirection.Down:
                if (Physics2D.OverlapBox(ringBlockDown.position, ringBlockedSize, 0, ringBlockLayer))
                    return;

                ringAngle = new Vector3(transform.rotation.x, transform.rotation.y, -90);
                throwdir += new Vector2(rangeHorizontal * Mathf.Sign(transform.rotation.y) * 2, -rangeVertical);
                break;
        }

        OrthogonalDirection savedThrowDirection = _playerFacing.PointingDirection;

        GetComponent<Animator>().SetTrigger("attack");

        Utility.InvokeLambda(() =>
        {
            tr.ThrowableReference = Instantiate(tr.ThrowablePrefab, (Vector2)transform.position + throwdir - new Vector2(0, 0.5f), Quaternion.Euler(ringAngle));
            tr.ThrowableReference.GetComponent<Ring>().SendRing(savedThrowDirection);
            AudioManager.instance.PlaySound("throw");
        }, 0.1f);
        
    }
}
