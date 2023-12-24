using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    [ColorUsage(true, true)]
    public Color _flashColour = Color.white;
    public float _flashTime = 0.25f;

    private SpriteRenderer[] _spriteRenderers;
    private Material[] _materials;

    private Coroutine _damageFlashCoroutine;

    private void Awake()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        Init();
    }

    private void Init()
    {
        _materials = new Material[_spriteRenderers.Length]; 

        // assign sprite renderer mats to _mats
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _materials[i] = _spriteRenderers[i].material;
        }
    }

    public void DoFlashEffect()
    {
        _damageFlashCoroutine = StartCoroutine(FlashTimer());
    }

    private IEnumerator FlashTimer()
    {
        SetFlashColour();

        float currentFlashAmount = 0f;
        float elapsedTime = 0f;
        while(elapsedTime < _flashTime)
        {
            // iterate elapsed time
            elapsedTime += Time.deltaTime;

            // lerp flash amount
            currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / _flashTime));
            SetFlashAmount(currentFlashAmount);

            yield return null;
        }
    }

    private void SetFlashColour()
    {
        // set color
        for (int i = 0; i < _materials.Length; i++)
        {
            _materials[i].SetColor("_FlashColour", _flashColour);
        }
    }

    private void SetFlashAmount(float amount)
    {
        for (int i = 0; i < _materials.Length; i++)
        {
            _materials[i].SetFloat("_FlashAmount", amount);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) { DoFlashEffect(); }
    }
}
