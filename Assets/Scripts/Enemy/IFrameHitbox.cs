using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IFrameHitbox : MonoBehaviour
{
    protected bool _hitboxActive = true;

    private void OnEnable()
    {
        EventManager.StartListening(EventStrings.PLAYER_HIT, OnPlayerHit);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventStrings.PLAYER_HIT, OnPlayerHit);
    }

    private void OnPlayerHit(Dictionary<string, object> payload)
    {
        StartCoroutine(DisableHitboxTime());
        
    }

    private IEnumerator DisableHitboxTime()
    {
        _hitboxActive = false;
        yield return new WaitForSeconds(Config.PlayerIFrameTime);
        _hitboxActive = true;
    }
}
