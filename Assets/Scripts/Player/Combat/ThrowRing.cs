using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (ThrowInputPressed())
        {
            CheckRingRecieved();

            if (HoldingRing)
            {
                HoldingRing = false;
                return true;
            }
        }
        return false;
    }

    private bool ThrowInputPressed()
    {
        if (throwInputFunction != null)
        {
            return throwInputFunction();
        }
        else
        {
            Utility.PrintWarn("Throw Input Function Not Found at " + RingName);
            return false;
        }
    }

    private void CheckRingRecieved()
    {
        if (RingReference == null)
        {
            HoldingRing = true;
        }
    }

    public ThrowableRing(string _RingName, GameObject _RingObject, ThrowInputFunction InputFunction) 
    {
        throwInputFunction = InputFunction;
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

    [Space(10f)]
    public Vector2 ringBlockedSize = new(0.49f, 0.03f);
    public LayerMask ringBlockLayer;

    private PlayerFacing pf;
    private List<ThrowableRing> ThrowableRingList = new();

    private void Awake()
    {
        pf = GetComponent<PlayerFacing>();

        ThrowableRingList.Add(new ThrowableRing("Left Ring", LeftRingObj, PlayerControls.GetAttackLeftPressed));
        ThrowableRingList.Add(new ThrowableRing("Right Ring", RightRingObj, PlayerControls.GetAttackRightPressed));
    }

    private void Update()
    {
        foreach (ThrowableRing tr in ThrowableRingList)
        {
            if (tr.CheckRingThrow())
            {
                Throw(tr);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(ringBlockFront.position, ringBlockedSize);
        Gizmos.DrawWireCube(ringBlockUp.position, ringBlockedSize);
    }

    private void Throw(ThrowableRing tr)
    {
        Vector2 throwdir = transform.position;
        float rangeHorizontal = 2.0f;
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

                throwdir += new Vector2(0, rangeVertical);
                break;

            case PlayerFacingDirection.Down:
                throwdir += new Vector2(0, -rangeVertical);
                break;
        }

        tr.RingReference = Instantiate(tr.RingObject, throwdir, Quaternion.identity);
        tr.RingReference.GetComponent<Ring>().SendRing(pf.GetFacingDirection());
    }
}
