using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashVFX : MonoBehaviour
{
    [SerializeField] private float slashVfxTime;

    private Dictionary<string, GameObject> slashes = new Dictionary<string, GameObject>();

    [SerializeField] private GameObject[] slashFX;

    private Coroutine lastCoroutine;

    private ICurrentAttackNodeProvider attackProvider;

    public void Initialize(ICurrentAttackNodeProvider _attackProvider)
    {
        attackProvider = _attackProvider;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var slashVFX in slashFX)
        {
            slashes[slashVFX.name] = slashVFX;
        }
    }

    //this is called in an animation event
    public void PlaySlash()
    {


        if (lastCoroutine != null)
        {
            StopCoroutine(lastCoroutine);
        }

        lastCoroutine = StartCoroutine(SlashCoroutine());
    }

    public void StopSlashAndCoroutine()
    {
        foreach (var slashVFX in slashes.Values)
        {
            slashVFX.SetActive(false);
        }

        StopAllCoroutines();
    }

    private IEnumerator SlashCoroutine()
    {
        foreach (var slashVFX in slashes.Values)
        {
            slashVFX.SetActive(false);
        }

        if(attackProvider.CurrentAttackNode != null)
        {
            slashes[attackProvider.CurrentAttackNode.slashAttackName].SetActive(true);
            yield return new WaitForSecondsRealtime(slashVfxTime);
            slashes[attackProvider.CurrentAttackNode.slashAttackName].SetActive(false);
        }

    }
}
