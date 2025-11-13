using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class AI_Monster : MonoBehaviour
{
    public GameObject[] List_Target_Position1Max;
    public NavMeshAgent agent;

    public MeshRenderer meshRenderer;
    public Color origine_color;
    float delay = 0.15f;

    private int VieMax = 100;
    private int Vie = 100;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        //origine_color = GetComponent<Color>();
        VieMax = Vie = 100;
    }

    private void Update()
    {
        //Permet de faire bouger l'AI_Monster
        if (Keyboard.current.numpad7Key.isPressed && List_Target_Position1Max[0]!=null)
        {
            print ("deplacement vers la position 1");
            agent.SetDestination(List_Target_Position1Max[0].transform.position);
        }
        if (Keyboard.current.numpad9Key.isPressed && List_Target_Position1Max[1] != null)
        {
            print("deplacement vers la position 2");
            agent.SetDestination(List_Target_Position1Max[1].transform.position);
        }
        //Permet que le monstre se prend des dégats
        if (Keyboard.current.numpad5Key.isPressed)
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int Damage)
    {
        int Resulte_vie = Vie - Damage;
        if (Resulte_vie <= 0)
        {
            print($"Resulte_vie: {Resulte_vie} VIE: {Vie}");
            meshRenderer.material.color = Color.black;
        }
        else
        {
            meshRenderer.material.color = Color.red;
            Vie -= Damage;
            print($"Resulte_vie: {Resulte_vie} VIE: {Vie}");
            Invoke("FlashStop", delay);
        }
    }
    private void FlashStop()
    {
        meshRenderer.material.color = origine_color;
    }
}
