using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ColliderSphere_ATK_Joueur : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Destroy(gameObject, 2);
    }
    //private void OnTriggerEnter(Collider other)
    private void OnTriggerStay(Collider other)
    {
        CS_Player cS_Player;

        if (other.gameObject.tag == "Player")
        {
            cS_Player = other.GetComponent<CS_Player>();
            if (cS_Player != null)
            {
                cS_Player.Take_Damage_player(1);
                Destroy(gameObject);
            }
        }
    }
    
}
