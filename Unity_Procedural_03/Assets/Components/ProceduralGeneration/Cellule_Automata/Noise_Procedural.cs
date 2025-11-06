using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using VTools.Grid;
using VTools.ScriptableObjectDatabase;

[CreateAssetMenu(menuName = "Procedural Generation Method/Noise_Automata")]
public class Noise_Procedural : ProceduralGenerationMethod
{
    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        var FondGrass = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Grass");
        var FondSable = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Sand");
        var FondEau  = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Water");
        var FondForet  = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Foret");

        int Scale_Map = Random.Range(1234, 3000);
        FastNoiseLite noise = new FastNoiseLite(Scale_Map);
        //FastNoiseLite noise = new FastNoiseLite(GridGenerator._seed);
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);

        // Gather noise data
        float[,] noiseData = new float[Grid.Width, Grid.Lenght];

        for (int x = 0; x < Grid.Width; x++)
        {
            for (int y = 0; y < Grid.Lenght; y++)
            {
                noiseData[x, y] = noise.GetNoise(x, y);
                if (!Grid.TryGetCellByCoordinates(x, y, out var cell))
                {
                    Debug.LogError("Grille pas afficher");
                }
                if (noiseData[x, y] < 0.05)
                {
                    GridGenerator.AddGridObjectToCell(cell, FondForet, true); // ajoute un élément sur la grid
                }
                else if (noiseData[x, y] < 0.3)
                {
                    GridGenerator.AddGridObjectToCell(cell, FondGrass, true);
                }
                else if (noiseData[x, y] < 0.5)
                {
                    GridGenerator.AddGridObjectToCell(cell, FondSable, true);
                }
                else if (noiseData[x, y] <= 1)
                {
                    GridGenerator.AddGridObjectToCell(cell, FondEau, true);
                }

            }
            
        }
    }
}
