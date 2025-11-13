using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class AI_Monstre_Fuser : MonoBehaviour
{
    private int PV_Monstre = 25;

    GameObject TargetPlayer;
    CS_Player cS_Player;
    private float _speed = 3f;
    bool move = true;
    //bool Attache = false;

    Rigidbody _rb;

    public GameObject Prefable_ATK_Joueur;
    bool SecureAtkJoueur = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        cS_Player = FindObjectOfType<CS_Player>();
        TargetPlayer = GameObject.FindGameObjectWithTag("Player");
    }

    /*private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "Player" && PV_Monstre >=1)
        {
            cS_Player = other.GetComponent<CS_Player>();
            if (cS_Player != null)
            {
                cS_Player.Take_Damage_player(1);
                Attache =true;
                Invoke("Cadence_Degat_player", 2f);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Attache = false;
        }
    }*/

    private void Cadence_Degat_player()
    {
        GameObject projectile = Instantiate(Prefable_ATK_Joueur, transform.position, Quaternion.identity);
        SecureAtkJoueur = false;
    }

    private void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, TargetPlayer.transform.position);

        if (move && PV_Monstre>=1)
        {
            if (distance >= 2.5) 
            { 
                _rb.linearVelocity = transform.forward * _speed; 
            }
            else if (!SecureAtkJoueur)
            {
                SecureAtkJoueur = true;
                Invoke("Cadence_Degat_player", 2f);
            }
            transform.LookAt(TargetPlayer.transform);
        }
        else
        {
            _rb.linearVelocity = transform.forward * -(_speed / 3);
        }
    }

    public void TakeDamage(int damage)
    {
        
        if (cS_Player != null)
        {
            if (PV_Monstre - damage > 0)
            {
                PV_Monstre -= damage;
                move = false;
                Invoke("DontStuck", 2f);
                print($"PV RESTANT DU MONSTRE: {PV_Monstre}");
            }
            else if (PV_Monstre - damage <= 0)//(cS_Player != null && cS_Player.UI_Player != null)
            {
                PV_Monstre = 0;
                move = false;
                print($"PV RESTANT DU MONSTRE: {PV_Monstre}");
                cS_Player.Victoire_Joueur();
            }
        }
    }

    private void DontStuck()
    {
        move = true;
    }
}
