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
    private FirstPersonController Ref_Autre_Controller;

    public float Affiche_UI_Eau_sur_quelle_position;
    public GameObject Affiche_UI_Eau_;


    private float TempsFlash = 0;
    private int Restant_Munition_FusilPompe= 5;

    public float cadence = 0.5f;   // Temps entre chaque tir en secondes
    private float prochainTir = 0f;

    bool régénération_en_cour = false;
    bool Allumer;

    private void Start()
    {
        Ref_Autre_Controller = FindObjectOfType<FirstPersonController>();
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
        if (transform.position.y<= Affiche_UI_Eau_sur_quelle_position)
        {
            Affiche_UI_Eau_.SetActive(true);
        }
        else
        {
            Affiche_UI_Eau_.SetActive(false);
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
        else if (Resulte_vie <= 0) { PV_Joueur -= Damage; UI_Player.SetInteger("PV_joueur", 0); Ref_Autre_Controller.SetCursorVisibility(true);  Cursor.visible = true; } //<---------------------------------------A ajouter et a affiche la souri et click sur les bouton
        
        //if (!régénération_en_cour) { régénration_Pv_Joueur(); }
        régénération_en_cour = false;
        régénration_Pv_Joueur();
        print($"PV JOUEUR: {PV_Joueur}");
    }

    public void Victoire_Joueur()
    {
        print("C'EST BON");
        UI_Player.SetInteger("PV_joueur", 5);
        UI_Player.SetBool("Victoire", true);
        Ref_Autre_Controller.SetCursorVisibility(true);
        Cursor.visible = true;//<---------------------------------------A ajouter et a affiche la souri et click sur les bouton
    }


    private void régénration_Pv_Joueur()
    {
        print($"R1 {régénération_en_cour}");
        if (PV_Joueur >= 0 && PV_Joueur <= 3)
        {
            print($"R1 {régénération_en_cour}");
            if (régénération_en_cour && PV_Joueur <= 3)
            {
                PV_Joueur++;
                UI_Player.SetInteger("PV_joueur", PV_Joueur);
                print($"PV JOUEUR: {PV_Joueur} R3 {régénération_en_cour}");
                Invoke("régénration_Pv_Joueur", 5);
            }
            else if (régénération_en_cour && PV_Joueur >= 4)
            {
                PV_Joueur = 4;
                print($"PV JOUEUR: {PV_Joueur} R4 {régénération_en_cour}");
                UI_Player.SetInteger("PV_joueur", PV_Joueur);
                régénération_en_cour = false;

            }
            else if (!régénération_en_cour && PV_Joueur >= 1)
            {
                régénération_en_cour = true;
                Invoke("régénration_Pv_Joueur", 5);
            }
        }
        /*print($"R1 {régénération_en_cour}");
        if (PV_Joueur >= 0 && PV_Joueur <= 3)
        {
            print($"R1 {régénération_en_cour}");
            if (!régénération_en_cour)
            {
                régénération_en_cour = true;
                Invoke("régénration_Pv_Joueur", 5);
            }
            else if (régénération_en_cour && PV_Joueur <= 3)
            {
                PV_Joueur++;
                UI_Player.SetInteger("PV_joueur", PV_Joueur);
                print($"PV JOUEUR: {PV_Joueur} R3 {régénération_en_cour}");
                Invoke("régénration_Pv_Joueur", 5);
            }
            else if (régénération_en_cour && PV_Joueur >= 4)
            {
                PV_Joueur = 4;
                print($"PV JOUEUR: {PV_Joueur} R4 {régénération_en_cour}");
                UI_Player.SetInteger("PV_joueur", PV_Joueur);
                //régénération_en_cour = false;
            }
        }*/
        
    }
}
