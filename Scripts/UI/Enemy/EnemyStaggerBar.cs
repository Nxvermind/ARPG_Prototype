using UnityEngine;
using UnityEngine.UI;

public class EnemyStaggerBar : MonoBehaviour
{
    [SerializeField] private Image staggerBar;
    private EnemyParameters enemyParameters;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyParameters = GetComponentInParent<EnemyParameters>();
    }

    // Update is called once per frame
    void Update()
    {
        staggerBar.fillAmount = enemyParameters.currentStaggerValue/enemyParameters.maxStaggerValue;
    }
    void LateUpdate()
    {
        //para que la barra mire en dirección del jugador despues de haberse actualizado (es world space)
        transform.LookAt(Camera.main.transform);
    }
}
