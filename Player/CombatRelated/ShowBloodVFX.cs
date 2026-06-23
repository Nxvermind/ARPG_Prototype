using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBloodVFX : MonoBehaviour
{
    public List<GameObject> bloodFXFromRightAttack = new();
    public List<GameObject> bloodFXFromLeftAttack = new();
    private readonly List<GameObject> totalFX = new();

    private readonly List<GameObject> availableFX = new();

    private void Start()
    {
        foreach(var obj in bloodFXFromRightAttack)
        {
            if(!totalFX.Contains(obj)) totalFX.Add(obj);
        }

        foreach (var obj in bloodFXFromLeftAttack)
        {
            if(!totalFX.Contains(obj)) totalFX.Add(obj);
        }
    }

    public void SetPosition(Vector3 point)
    {
        transform.position = point;
    }

    public void ShowRandomBloodFX()
    {
        ShowAvailableBloodFX(totalFX);
    }

    public void ShowRandomBloodFX(AttackNode attackNode)
    {
        if(attackNode.attackDirection == AttackDirection.Left)
        {
            ShowAvailableBloodFX(bloodFXFromLeftAttack);
        }
        else if(attackNode.attackDirection == AttackDirection.Right)
        {
            ShowAvailableBloodFX(bloodFXFromRightAttack);
        }
        else
        {
            ShowAvailableBloodFX(totalFX);
        }
    }

    private void ShowAvailableBloodFX(List<GameObject> bloodList)
    {
        availableFX.Clear();

        foreach (var fx in bloodList)
        {
            if (!fx.activeSelf)
            {
                availableFX.Add(fx);
            }
        }

        if (availableFX.Count > 0)
        {
            int rnd = Random.Range(0, availableFX.Count);

            StartCoroutine(Show(availableFX[rnd]));
            return;
        }

        CreateAdditionalBloodFX(bloodList);
    }

    private void CreateAdditionalBloodFX(List<GameObject> bloodList)
    {
        int selectRnd = Random.Range(0, bloodList.Count);
        GameObject obj = Instantiate(bloodList[selectRnd], transform.parent);
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        bloodList.Add(obj);

        StartCoroutine(Show(obj));

    }

    private IEnumerator Show(GameObject fx)
    {
        fx.SetActive(true);
        fx.transform.SetParent(null);

        yield return new WaitForSeconds(.8f);

        fx.SetActive(false);
        fx.transform.SetParent(transform);
        fx.transform.localPosition = Vector3.zero;
    }
}
