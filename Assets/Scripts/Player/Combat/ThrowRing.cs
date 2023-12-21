using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowRing : MonoBehaviour
{
    public GameObject ring;
    private PlayerFacing pf;

    private void Awake()
    {
        pf = GetComponent<PlayerFacing>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerControls.GetAttackRightPressed() || PlayerControls.GetAttackLeftPressed())
        {
            Throw();
        }
    }

    private void Throw()
    {
        Vector2 throwdir = transform.position;
        float rangeHorizontal = 2.0f;
        float rangeVertical = 2.0f;

        switch (pf.GetFacingDirection())
        {
            case PlayerFacingDirection.Left:
                throwdir += new Vector2(-rangeHorizontal, 0);
                break;

            case PlayerFacingDirection.Right:
                throwdir += new Vector2(rangeHorizontal, 0);
                break;

            case PlayerFacingDirection.Up:
                throwdir += new Vector2(0, rangeVertical);
                break;

            case PlayerFacingDirection.Down:
                throwdir += new Vector2(0, -rangeVertical);
                break;
        }

        GameObject _ring = Instantiate(ring, throwdir, Quaternion.identity);
        _ring.GetComponent<Ring>().SendRing(pf.GetFacingDirection());
    }
}
