using UnityEngine;


public class CS_Player : MonoBehaviour
{
    public int PV_JoueurMax { get; set; }
    public int PV_Joueur { get; set; }
    public int Recharge_Pile { get; set; }
    public int Recharge_Munition{ get; set; }

    private float TempsFlash = 0;
    private int Fire= 0;
    bool Allumer;

    private void Start()
    {
        PV_JoueurMax = PV_Joueur = 4;
        Recharge_Pile = 1;
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
                if (Recharge_Munition >= 1)
                {
                    Recharge_Munition -= 1;
                    Fire = 1; 
                }else print(" J'ai pas de recharge de Munition");
                break;

            case "Pile":
                if (Recharge_Pile >= 1)
                {
                    Recharge_Pile -= 1;
                    TempsFlash = 100.0f;
                }else print(" J'ai pas de recharge de pile");
                    break;
        }
        
    }

    public void Take_Damage_player(int Damage)
    {
        int Resulte_vie = PV_Joueur - Damage;
        if (Resulte_vie == 3) { }
        else if (Resulte_vie == 2) { }
        else if (Resulte_vie == 1) { }
        else if (Resulte_vie <= 0) { }
        else { }
    }
}
