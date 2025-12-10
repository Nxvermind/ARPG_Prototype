using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBar : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    private EnemyParameters enemyParameters;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyParameters = GetComponentInParent<EnemyParameters>();   
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.fillAmount = enemyParameters.currentHp/enemyParameters.maxHp;
    }
    
    void LateUpdate()
    {
        //para que la barra mire en dirección del jugador despues de haberse actualizado (es world space)
        transform.LookAt(Camera.main.transform);
    }
}
