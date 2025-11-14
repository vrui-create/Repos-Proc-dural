using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class AI_Monstre_Fuser : MonoBehaviour
{
    private int PV_Monstre = 25;

    GameObject TargetPlayer;
    CS_Player cS_Player;
    private Animator animator;
    private float _speed = 3f;
    bool move = true;
    //bool Attache = false;

    Rigidbody _rb;

    public AudioClip[] mySoundClip;  // Le clip audio que tu veux jouer
    private AudioSource audioSource_ATK;  // L'AudioSource qui va jouer le son
    private AudioSource audioSource_Idle;  // L'AudioSource qui va jouer le son
    private AudioSource audioSource_HIT;  // L'AudioSource qui va jouer le son

    public GameObject Prefable_ATK_Joueur;
    bool SecureAtkJoueur = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        cS_Player = FindObjectOfType<CS_Player>();
        TargetPlayer = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();

        audioSource_Idle = gameObject.AddComponent<AudioSource>();
        audioSource_Idle.clip = mySoundClip[0];

        audioSource_HIT = gameObject.AddComponent<AudioSource>();
        audioSource_HIT.clip = mySoundClip[1];

        audioSource_ATK = gameObject.AddComponent<AudioSource>();
        audioSource_ATK.clip = mySoundClip[2];
        SpawnSound("idle");
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

    public void SpawnSound(string TypeSound)
    {
        switch (TypeSound)
        {
            case "idle":
                audioSource_Idle.loop = true;
                audioSource_Idle.Play();
                break;
            case "hit":
                audioSource_HIT.Play();
                break;
            case "ATK":
                audioSource_ATK.Play();
                break;
        }
    }

    public void destroySound(string TypeSound)
    {
        switch (TypeSound)
        {
            case "idle":
                audioSource_Idle.Stop();
                break;
            case "hit":
                audioSource_HIT.Stop();
                break;
            case "ATK":
                audioSource_ATK.Stop();
                break;
            case "all":
                audioSource_Idle.Stop();
                audioSource_HIT.Stop();
                audioSource_ATK.Stop();
                break;
        }
    }







    public void Cadence_Degat_player()
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
                if (animator != null)
                {
                    animator.SetTrigger("ATK");
                    Invoke("Cadence_Degat_player", 2.8f);
                }
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
                if (animator != null)
                {
                    animator.SetTrigger("STUN");
                    animator.Play("_A_Skelette_A_Phantom_Stun");
                }
                //Invoke("DontStuck", 2f);
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
