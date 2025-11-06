using System.Collections.Generic;
using System.Threading;
using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VTools.Grid;
using VTools.RandomService;
using VTools.ScriptableObjectDatabase;
using VTools.Utility;

[CreateAssetMenu(menuName = "Procedural Generation Method/Cellule_Automata")]
public class Cellule_Automata : ProceduralGenerationMethod
{
    public float noiseDensity;
    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        Grid_Génération();
    }
    private void Grid_Génération()
    {
        for (int x = 0; x < Grid.Width; x++)
        {
            for (int y = 0; y < Grid.Lenght; y++)
            {

                //if (!Grid.TryGetCellByCoordinates) (x, y, out var cell)
                /*if (RandomService.Chance(noiseDensity))
                {
                    AddTileToCell(cell )
                }*/

    }
}
    }
   
}
