using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public class Throwable
{
    [SerializeField]
    public Sprite ThrowableIcon;

    [SerializeField]
    public GameObject ThrowablePrefab;
    public GameObject ThrowableReference { get; set; }
    private bool _isConsumable = false;

    public bool CanThrow()
    {
        return ThrowableReference == null;
    }

    public bool IsConsumable()
    {
        return _isConsumable;
    }

    public bool IsNull()
    {
        return ThrowablePrefab == null;
    }

    public Throwable(GameObject _throwable, bool _isConsumable = false) 
    {
        ThrowablePrefab = _throwable;
        this._isConsumable= _isConsumable;
    }
}

public class ThrowRing : MonoBehaviour
{
    [Header("Throwables")]
    [SerializeField] private List<Throwable> _throwableList = new();

    [Header("Throw Blockers")]
    public Transform ringBlockFront;
    public Transform ringBlockUp;
    public Transform ringBlockDown;

    [Space(5f)]
    public Vector2   ringBlockedSize = new(0.49f, 0.03f);
    public LayerMask ringBlockLayer;

    [Space(10f)]
    public float RingThrowCooldown;
    public float RingSwapCooldown;

    [Header("UI")]
    [SerializeField] private List<Image> _throwableImageSlot = new();

    private bool _canThrow = true;
    private bool _canSwap  = true;

    private PlayerFacing    _playerFacing;
    private Animator        _playerAnimator;

    private void Awake()
    {
        _playerFacing = GetComponent<PlayerFacing>();
        _playerAnimator = GetComponent<Animator>();

        SetThrowableImages();
    }

    private void SetThrowableImages()
    {
        if (_throwableList.Count > _throwableImageSlot.Count)
        {
            Logger.PrintWarn("There are more throwable objects than available image slots!");
        }

        for (int i = 0; i < _throwableImageSlot.Count; i++)
        {
            if (_throwableList[i].ThrowableIcon != null)
                _throwableImageSlot[i].sprite = _throwableList[i].ThrowableIcon;
        }
    }

    public void SwapThrowableOrderRight(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started || !_canSwap)
            return;

        // Ensure only 1 or less throwable is out
        int ringsOut = 0;
        foreach (Throwable throwable in _throwableList)
        {
            if (!throwable.CanThrow())
            {
                ringsOut++;
            }

            if (ringsOut > 1)
            {
                return;
            }
        }

        ShiftThrowableUI(-1);
        Throwable temp = _throwableList[_throwableList.Count - 1];
        for (int i = _throwableList.Count - 1; i > 0; i--)
        {
            _throwableList[i] = _throwableList[i - 1];
        }
        _throwableList[0] = temp;
    }

    public void SwapThrowableOrderLeft(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started || !_canSwap)
            return;

        // Ensure only 1 or less throwable is out
        int ringsOut = 0;
        foreach (Throwable throwable in _throwableList)
        {
            if (!throwable.CanThrow())
            {
                ringsOut++;
            }

            if (ringsOut > 1)
            {
                return;
            }
        }

        ShiftThrowableUI(1);
        Throwable temp = _throwableList[0];
        for (int i = 1; i < _throwableList.Count; i++)
        {
            _throwableList[i - 1] = _throwableList[i];
        }
        _throwableList[_throwableList.Count-1] = temp;
    }

    private void ShiftThrowableUI(int direction)
    {
        StartCoroutine(SwapRingCooldown());

        List<Vector3> positionLists = new();
        foreach (Image slot in _throwableImageSlot)
        {
            positionLists.Add(slot.GetComponent<RectTransform>().position);
        }

        if (direction == -1)
        {
            // Shift array left
            Vector3 temp = positionLists[0];
            for (int i = 1; i < positionLists.Count; i++)
            {
                positionLists[i - 1] = positionLists[i];
            }
            positionLists[positionLists.Count - 1] = temp;

            // Move UI elements            
            for (int i = 0; i < _throwableImageSlot.Count; i++) 
            {
                LeanTween.move(_throwableImageSlot[i].gameObject, positionLists[i], 0.15f).setEaseInSine().setOnComplete(() => { AudioManager.instance.PlaySound("click"); });
            }
        }

        if (direction == 1)
        {
            // Shift array right
            Vector3 temp = positionLists[positionLists.Count - 1];
            for (int i = positionLists.Count - 1; i > 0; i--)
            {
                positionLists[i] = positionLists[i - 1];
            }
            positionLists[0] = temp;

            // Move UI elements
            for (int i = 0; i < _throwableImageSlot.Count; i++)
            {
                LeanTween.move(_throwableImageSlot[i].gameObject, positionLists[i], 0.15f).setEaseInSine().setOnComplete(() => { AudioManager.instance.PlaySound("click"); });
            }
        }
        
    }

    public void ThrowObject(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
            return;

        for (int i = 0; i < _throwableList.Count-1; i++)
        {
            if (_throwableList[i].CanThrow() && !_throwableList[i].IsNull())
            {
                Throw(_throwableList[i]);
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
            StartCoroutine(ThrowRingCooldown());
        else
            return;

        Vector2 throwPos = Vector2.zero;
        Vector3 ringAngle = Vector3.zero;
        float rangeHorizontal = 1.0f;
        float rangeVertical = 2.0f;

        switch (_playerFacing.PointingDirection)
        {
            case OrthogonalDirection.Left:
                if (Physics2D.OverlapBox(ringBlockFront.position, ringBlockedSize, 0, ringBlockLayer))
                    return;

                throwPos += new Vector2(-rangeHorizontal, 0);
                break;

            case OrthogonalDirection.Right:
                if (Physics2D.OverlapBox(ringBlockFront.position, ringBlockedSize, 0, ringBlockLayer))
                    return;

                throwPos += new Vector2(rangeHorizontal, 0);
                break;

            case OrthogonalDirection.Up:
                if (Physics2D.OverlapBox(ringBlockUp.position, ringBlockedSize, 0, ringBlockLayer))
                    return;

                ringAngle = new Vector3(transform.rotation.x, transform.rotation.y, 90);
                throwPos += new Vector2(rangeHorizontal * Mathf.Sign(transform.rotation.y) * 2, rangeVertical);
                break;

            case OrthogonalDirection.Down:
                if (Physics2D.OverlapBox(ringBlockDown.position, ringBlockedSize, 0, ringBlockLayer))
                    return;

                ringAngle = new Vector3(transform.rotation.x, transform.rotation.y, -90);
                throwPos += new Vector2(rangeHorizontal * Mathf.Sign(transform.rotation.y) * 2, -rangeVertical);
                break;
        }

        StartCoroutine(ThrowRingDelay(tr, throwPos, ringAngle, _playerFacing.PointingDirection));
    }

    private IEnumerator ThrowRingCooldown()
    {
        _canThrow = false;
        yield return new WaitForSeconds(RingThrowCooldown);
        _canThrow = true;
    }

    private IEnumerator SwapRingCooldown()
    {
        _canSwap = false;
        yield return new WaitForSeconds(RingSwapCooldown);
        _canSwap = true;
    }

    private IEnumerator ThrowRingDelay(Throwable tr, Vector2 throwPos, Vector3 ringAngle, OrthogonalDirection throwDir)
    {
        _playerAnimator.SetTrigger("attack");
        yield return new WaitForSeconds(_playerAnimator.GetCurrentAnimatorStateInfo(0).length * _playerAnimator.GetCurrentAnimatorStateInfo(0).speed);

        tr.ThrowableReference = Instantiate(tr.ThrowablePrefab, (Vector2)transform.position + throwPos - new Vector2(0, 0.5f), Quaternion.Euler(ringAngle));
        tr.ThrowableReference.GetComponent<Ring>().SendRing(throwDir);
        AudioManager.instance.PlaySound("throw");
    }
}
