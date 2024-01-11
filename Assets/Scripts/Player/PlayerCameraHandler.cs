using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCameraHandler : MonoBehaviour
{
    [SerializeField]
    private CameraFollowObject FollowObject;

    private Rigidbody2D _RB;

    private void Awake()
    {
        _RB = GetComponent<Rigidbody2D>();

        if (FollowObject != null )
        {
            Logger.PrintErr("Player camera handler does not have a Follow Object");
        }
    }

    private void Update()
    {
        CheckFallCameraChange();
    }

    public void CallTurn()
    {
        FollowObject.CallTurn();
    }

    private void CheckFallCameraChange()
    {
        // if falling past certain speed threshold
        if (_RB.velocity.y < CameraManager.instance._fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        // if standing still or moving up
        if (_RB.velocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            // reset so it can be called again
            CameraManager.instance.LerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        }
    }
}
