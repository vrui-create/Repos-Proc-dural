using UnityEngine;
using UnityEngine.InputSystem;


public class CS_Player : MonoBehaviour
{
    public int PV_JoueurMax { get; set; }
    public int PV_Joueur { get; set; }
    public int Recharge_Pile { get; set; }
    public int Recharge_Munition{ get; set; }
    
    public GameObject Prefable_Projectile;
    public Transform cam;
    public Animator UI_Player;

    private float TempsFlash = 0;
    private int Restant_Munition_FusilPompe= 5;

    public float cadence = 0.5f;   // Temps entre chaque tir en secondes
    private float prochainTir = 0f;

    bool Allumer;

    private void Start()
    {
        PV_JoueurMax = PV_Joueur = 4;
        Recharge_Pile = 1;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame && Time.time >= prochainTir && Restant_Munition_FusilPompe>= 1)
        {
            spawnProjectile();
            Restant_Munition_FusilPompe--;
            prochainTir = Time.time + cadence;
        }
        if (Mouse.current.rightButton.wasReleasedThisFrame && Time.time >= prochainTir && Restant_Munition_FusilPompe >= 1)
        {
            UI_Player.SetInteger("PV_joueur", 2); 
            print("succes"); 
        }
    }


    private void spawnProjectile()
    {

        Vector3 spawnPosition = cam.transform.position;
        Vector3 shootDirection = cam.transform.forward;
        GameObject projectile = Instantiate(Prefable_Projectile, spawnPosition, Quaternion.identity);
        projectile.transform.forward = shootDirection;
    }


    public void UseLampe()
    {
        if (TempsFlash >= 1 && Allumer)
        {
            TempsFlash -= 1;
            Invoke("perte_puissance_lampe", 0.15f);
        }
        else print("Je n'est plus de pile");
    }
   
    public void Recharge(string NameOption)
    {
        switch(NameOption)
        {
            case "Munition":
                Restant_Munition_FusilPompe += 1;
                print($"MUNITION RECHARGE +1 Balle RESTANT {Restant_Munition_FusilPompe}");
                break;

            case "Flash":
                if (Recharge_Pile >= 1)
                {
                    Recharge_Pile -= 1;
                    TempsFlash = 100.0f;
                }else print(" J'ai pas de recharge de pile");
                break;
            case "Pile":
                Recharge_Pile++;
                    break;
        }
        
    }

    public void Take_Damage_player(int Damage)
    {
        print("take damage success");

        int Resulte_vie = PV_Joueur - Damage;
        if (Resulte_vie == 3) { PV_Joueur -= Damage; UI_Player.SetInteger("PV_joueur", 3); }
        else if (Resulte_vie == 2) { PV_Joueur -= Damage;  UI_Player.SetInteger("PV_joueur", 2); }
        else if (Resulte_vie == 1) { PV_Joueur -= Damage; UI_Player.SetInteger("PV_joueur", 1); }
        else if (Resulte_vie <= 0) { PV_Joueur -= Damage; UI_Player.SetInteger("PV_joueur", 0); }
    }
}
