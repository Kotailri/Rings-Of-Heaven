using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public delegate bool ThrowInputFunction();

public class ThrowableRing
{
    private readonly string RingName;
    public GameObject RingObject { get; set; }
    public ThrowInputFunction throwInputFunction;
    public GameObject RingReference { get; set; }

    public bool HoldingRing { get; set; }

    public bool CheckRingThrow()
    {
        CheckRingRecieved();

        if (HoldingRing)
        {
            HoldingRing = false;
            return true;
        }
        return false;
    }

    private void CheckRingRecieved()
    {
        if (RingReference == null)
        {
            HoldingRing = true;
        }
    }

    public ThrowableRing(string _RingName, GameObject _RingObject) 
    {
        RingName = _RingName;
        RingObject = _RingObject;
        HoldingRing = true;
    }
}

public class ThrowRing : MonoBehaviour
{
    public GameObject LeftRingObj;
    public GameObject RightRingObj;

    [Space(10f)]
    public Transform ringBlockFront;
    public Transform ringBlockUp;
    public Transform ringBlockDown;

    [Space(10f)]
    public Vector2 ringBlockedSize = new(0.49f, 0.03f);
    public LayerMask ringBlockLayer;

    [Space(10f)]
    public float ringThrowCooldown;
    private bool canThrow = true;

    private PlayerFacing pf;
    private List<ThrowableRing> ThrowableRingList;

    private void Awake()
    {
        pf = GetComponent<PlayerFacing>();

        ThrowableRingList = new()
        {
            new ThrowableRing("Left Ring", LeftRingObj),
            new ThrowableRing("Right Ring", RightRingObj)
        };
    }

    public void ThrowLeftRing(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
            return;

        ThrowableRing tr = ThrowableRingList[0];
        if (tr.CheckRingThrow())
        {
            Throw(tr);
        }
    }

    public void ThrowRightRing(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
            return;

        ThrowableRing tr = ThrowableRingList[1];
        if (tr.CheckRingThrow())
        {
            Throw(tr);
        }
    }

    public void ThrowBothRings(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
            return;

        foreach (ThrowableRing throwableRing in ThrowableRingList)
        {
            if (throwableRing.CheckRingThrow())
            {
                Throw(throwableRing);
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

    private void Throw(ThrowableRing tr)
    {
        if (canThrow)
        {
            canThrow = false;
            Utility.InvokeLambda(() => { canThrow = true; }, ringThrowCooldown);
        }
        else
        {
            return;
        }

        Vector2 throwdir = Vector2.zero;
        Vector3 ringAngle = Vector3.zero;
        float rangeHorizontal = 1.0f;
        float rangeVertical = 2.0f;



        switch (pf.GetFacingDirection())
        {
            case PlayerFacingDirection.Left:
                if (Physics2D.OverlapBox(ringBlockFront.position, ringBlockedSize, 0, ringBlockLayer))
                    return;

                throwdir += new Vector2(-rangeHorizontal, 0);
                break;

            case PlayerFacingDirection.Right:
                if (Physics2D.OverlapBox(ringBlockFront.position, ringBlockedSize, 0, ringBlockLayer))
                    return;

                throwdir += new Vector2(rangeHorizontal, 0);
                break;

            case PlayerFacingDirection.Up:
                if (Physics2D.OverlapBox(ringBlockUp.position, ringBlockedSize, 0, ringBlockLayer))
                    return;

                ringAngle = new Vector3(transform.rotation.x, transform.rotation.y, 90);
                throwdir += new Vector2(rangeHorizontal * Mathf.Sign(transform.rotation.y) * 2, rangeVertical);
                break;

            case PlayerFacingDirection.Down:
                if (Physics2D.OverlapBox(ringBlockDown.position, ringBlockedSize, 0, ringBlockLayer))
                    return;

                ringAngle = new Vector3(transform.rotation.x, transform.rotation.y, -90);
                throwdir += new Vector2(rangeHorizontal * Mathf.Sign(transform.rotation.y) * 2, -rangeVertical);
                break;
        }

        PlayerFacingDirection savedThrowDirection = pf.GetFacingDirection();

        GetComponent<Animator>().SetTrigger("attack");

        Utility.InvokeLambda(() =>
        {
            tr.RingReference = Instantiate(tr.RingObject, (Vector2)transform.position + throwdir - new Vector2(0, 1f), Quaternion.Euler(ringAngle));
            tr.RingReference.GetComponent<Ring>().SendRing(savedThrowDirection);
            AudioManager.instance.PlaySound("throw");
        }, 0.1f);
        
    }
}
