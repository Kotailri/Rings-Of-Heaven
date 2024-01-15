using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultRing : Ring, IRingCatchable, IRingReturn
{
    private int   ringDamage = 1;
    private float ringRange  = 15.0f;
    private float ringSpeed  = 45.0f;

    private bool catchable = false;
    private bool returning = false;

    private Vector2 targetPosition;
    private GameObject player;

    private void Awake()
    {
        targetPosition = Vector2.positiveInfinity;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override int GetRingDamage()
    {
        return ringDamage;
    }

    public override float GetRingSpeed()
    {
        return ringSpeed;
    }

    public override void SendRing(OrthogonalDirection direction)
    {
        Vector2 v = Vector2.zero;
        switch (direction)
        {
            case OrthogonalDirection.Left:
                v = new(-ringRange, 0);
                break;

            case OrthogonalDirection.Right:
                v = new(ringRange, 0);
                break;

            case OrthogonalDirection.Up:
                v = new(0, ringRange);
                break;

            case OrthogonalDirection.Down:
                v = new(0, -ringRange);
                break;
        }
        targetPosition = (Vector2) player.transform.position + v;
    }

    public bool IsCatchable()
    {
        return catchable;
    }

    public bool IsReturning()
    {
        return returning;
    }

    public void Return()
    {
        returning = true;
        targetPosition = (Vector2) player.transform.position - new Vector2(0,0.5f);
    }

    private new void OnTriggerStay2D(Collider2D collision)
    {
        base.OnTriggerStay2D(collision);

        if (!IsReturning() && collision.gameObject.TryGetComponent(out TagManager tagManager))
        {
            if (tagManager.IsOfTag(Tags.BouncesRing))
            {
                catchable = true;
                Return();
            }
        }

        if (IsReturning() && collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!returning)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, GetRingSpeed() * Time.deltaTime);
            if ((Vector2) transform.position == targetPosition)
            {
                catchable = true;
                Return();
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, GetRingSpeed() * Time.deltaTime);
        }
    }
}
