using System.Threading;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AI_Monstre_Fuser : MonoBehaviour
{
    public int PV = 25;

    public Transform target;
    CS_Player cS_Player;
    private float _speed = 3f;
    bool move = true;
    Rigidbody _rb;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            cS_Player = other.GetComponent<CS_Player>();
            if (cS_Player != null)
            {
                cS_Player.Take_Damage_player(1);
                Invoke("Cadence_Degat_player", 2f);
            }
        }
        /*else { print(other); }*/
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            cS_Player = null;
        }
    }

    private void Cadence_Degat_player()
    {
        if (cS_Player != null)
        {
            cS_Player.Take_Damage_player(1);
            Invoke("Cadence_Degat_player", 2f);
        }
    }


    private void FixedUpdate()
    {
        if (move)
        {
            _rb.linearVelocity = transform.forward * _speed;
            transform.LookAt(target);
        }
        else
        {
            _rb.linearVelocity = transform.forward * -(_speed / 3);
        }
    }

    public void TakeDamage(int damage)
    {
       if (PV - damage >= 1)
        {
            PV -= damage;
            move = false;
            Invoke("DontStuck", 2f);
            print($"PV RESTANT DU MONSTRE: {PV}");
        }
        else
        {
            print("FIN GAME");
        }
    }

    private void DontStuck()
    {
        move = true;
    }
}
