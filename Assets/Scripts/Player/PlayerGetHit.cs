using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGetHit : MonoBehaviour
{
    [SerializeField] private float _knockbackTime;
    [SerializeField] private float _knockbackForce;

    public SpriteRenderer PlayerSprite;

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
        ApplyHit((int)payload["hitDamage"], new Vector2((float)payload["hitPositionX"], (float)payload["hitPositionY"]));
    }

    public void ApplyHit(int damage, Vector2 hitPosition, float force = 0, float time=0)
    {
        if (force == 0 && time == 0)
        {
            force = _knockbackForce;
            time = _knockbackTime;
        }

        EventManager.TriggerEvent(EventStrings.PLAYER_KNOCKED_BACK, new Dictionary<string, object> {
                { "force", force }, { "time", time }, { "hitPositionX", hitPosition.x }, { "hitPositionY", hitPosition.y } });

        StartCoroutine(ApplyIFrames(Config.PlayerIFrameTime));
        EventManager.TriggerEvent(EventStrings.PLAYER_DAMAGED, new Dictionary<string, object> { { "amount", damage } });
    }

    private IEnumerator ApplyIFrames(float duration)
    {
        PlayerSprite.color = new Color(1, 0, 0, 0.5f);
        yield return new WaitForSeconds(duration);
        PlayerSprite.color = new Color(1, 1, 1, 1);
    }
}
