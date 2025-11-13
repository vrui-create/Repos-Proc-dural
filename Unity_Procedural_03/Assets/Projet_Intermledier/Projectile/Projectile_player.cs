using UnityEngine;

public class Projectile_player : MonoBehaviour
{
    Rigidbody _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 5);
    }

    void Update()
    {
        _rb.linearVelocity = transform.forward * 10;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Monster")
        {
            AI_Monstre_Fuser monster = other.GetComponent<AI_Monstre_Fuser>();
            if (monster != null)
            {
                monster.TakeDamage(1);
                Destroy(gameObject); // détruire le projectile après impact
            }
        }
    }
    
}
