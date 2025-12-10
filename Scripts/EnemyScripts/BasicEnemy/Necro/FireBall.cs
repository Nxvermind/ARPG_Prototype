using System.Collections;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public Transform target;

    public Transform fireballOrigin;

    public float fireballDamage;
    public float speed;

    private Rigidbody rb;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();

        transform.parent = null;

        ShootingFireball();

        StartCoroutine(Cor());
    }

    private void DisableFireball()
    {
        transform.SetParent(fireballOrigin);
        gameObject.SetActive(false);

        transform.position = fireballOrigin.position;
    }

    public void ShootingFireball()
    {
        transform.LookAt(target);

        rb.linearVelocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerHurtbox"))
        {
            DisableFireball();

            IDamageable damageable = other.GetComponentInParent<IDamageable>();

            damageable.TakeDamage(fireballDamage, true);
        }
    }

    IEnumerator Cor()
    {
        yield return new WaitForSecondsRealtime(3);

        DisableFireball();
    }
}
