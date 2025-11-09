using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class AI_Monster : MonoBehaviour
{
    public GameObject[] List_Target_Position1Max;
    public NavMeshAgent agent;

    private void Update()
    {
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
    }
}
