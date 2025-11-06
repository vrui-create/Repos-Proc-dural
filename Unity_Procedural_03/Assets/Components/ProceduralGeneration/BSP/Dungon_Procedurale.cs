using System.Collections.Generic;
using System.Threading;
using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VTools.Grid;
using VTools.RandomService;
using VTools.ScriptableObjectDatabase;
using VTools.Utility;

[CreateAssetMenu(menuName = "Procedural Generation Method/Dungon Procedural")]// 

public class Dungon_Procedurale : ProceduralGenerationMethod
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        // Your procedural generation logic here
    }
}

public class  Node_Dungon
{
    public Node_Dungon()
    {

    }
}
