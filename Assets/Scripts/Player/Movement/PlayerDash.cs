using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    public float DashForce;
    public float DashCooldown;

    [Space(10)]
    public ParticleSystem DashParticle;
    public ParticleSystem CooldownIndicationParticle;

    private Rigidbody2D RB;
    private PlayerMovement pm;

    private float blockedMovementTime = 0.1f;

    private bool canDash = true;

    private Controls controls;

    private void Awake()
    {
        controls = new Controls();
        controls.Gameplay.Dash.started += ctx => Dash();

        controls.Gameplay.Enable();

        RB = GetComponent<Rigidbody2D>();
        pm = GetComponent<PlayerMovement>();
    }

    public void Dash()
    {
        if (PlayerUnlocks.isDashUnlocked && canDash)
        {
            Vector2 dashDirection = Vector2.zero;
            if (pm.facing == PlayerRBFacingDirection.Left)
            {
                dashDirection = Vector2.left;
            }

            if (pm.facing == PlayerRBFacingDirection.Right)
            {
                dashDirection = Vector2.right;
            }

            RB.velocity = Vector2.zero;
            RB.AddForce(dashDirection * DashForce, ForceMode2D.Impulse);

            DashParticle.Play();

            StartCoroutine(WaitDashCooldown());
            StartCoroutine(BlockMovement());
        }
    }

    private IEnumerator WaitDashCooldown()
    {
        canDash = false;
        yield return new WaitForSecondsRealtime(DashCooldown);
        canDash = true;
        CooldownIndicationParticle.Play();
    }

    private IEnumerator BlockMovement()
    {
        pm.ToggleMovement(false);
        yield return new WaitForSecondsRealtime(blockedMovementTime);
        pm.ToggleMovement(true);
    }
}
