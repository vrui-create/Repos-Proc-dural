using UnityEngine;
using UnityEngine.InputSystem;

public class Iteam_Collider : MonoBehaviour
{
    private bool Block_input = false;
    public GameObject TargetDestroy;
    public string Iteam_id;
    public CS_Player Player_CS;

    public GameObject Image_interaction_E;


    private void OnTriggerEnter(Collider other)//permet de détecter si un objet entre en collision avec le collider
    {
        if (other.gameObject.tag == "Player")//si l'objet qui entre en collision est le joueur
        {
            print("entrer en collision avec un iteam");
            Block_input = true;
            OnOFF_UE_Bouton_E(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")//si l'objet qui entre en collision est le joueur
        {
            print("sortir de la collision avec un iteam");
            Block_input = false;
            OnOFF_UE_Bouton_E(false);
        }
    }

    private void Update()
    {
        if (Keyboard.current.eKey.isPressed && Block_input == true)
        {
            switch(Iteam_id)
                {
                case "Pile":
                    Player_CS.Recharge("Pile");
                    OnOFF_UE_Bouton_E(false);
                    Destroy(TargetDestroy);
                    break;
                case "Soin":
                    print("Soin");
                    OnOFF_UE_Bouton_E(false);
                    Destroy(TargetDestroy);
                    break;
                case "Munition":
                    Player_CS.Recharge("Munition");
                    OnOFF_UE_Bouton_E(false);
                    Destroy(TargetDestroy);
                    break;
                default:
                    print("Iteam inconnu");
                    break;
            }
        }
    }
    public void OnOFF_UE_Bouton_E(bool interupteur)
    {
        if (interupteur) Image_interaction_E.SetActive(true);
        else Image_interaction_E.SetActive(false);
           
    }
}
