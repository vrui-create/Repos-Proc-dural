using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using VTools.Grid;
using VTools.ScriptableObjectDatabase;

// ce code permet de créer un menu déroulant dans unity pour choisir le type de génération procédurale
[CreateAssetMenu(menuName = "Procedural Generation Method/Noise_Automata")]
// au lieu de hériter de monobehaviour on hérite de ProceduralGenerationMethod, elle nous permet d'accéder à la grille et au générateur de grille
public class Noise_Procedural : ProceduralGenerationMethod 
{
    //En utiliser protected override async UniTask ApplyGeneration(CancellationToken cancellationToken) permetant d'appliquer la génération procédurale
    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        //J'ai utiliser c'est variables pour définir les différents types de terrain que je vais utiliser pour générer la map
        //tout fois il y a des méthode plus optimisé pour récupérer des ScriptableObject mais tant que ça fonctionne  je ne touche plus
        var FondGrass = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Grass");
        var FondSable = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Sand");
        var FondEau  = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Water");
        var FondForet  = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Foret");

        // j'ai définit une variable permetant d'utiliser l'aléatoire pour modifier l'emplacement du terrain
        int Scale_Map = Random.Range(1234, 3000);

        FastNoiseLite noise = new FastNoiseLite(Scale_Map);
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);

        //Le Grid.width permet compter la Largeur d'une grille.
        //Le Grid.Lenght c'est la taille max d'une grille
        float[,] noiseData = new float[Grid.Width, Grid.Lenght];

        //Ce code ci dessous permet d'utiliser un sound, celan l'intensiter du song elle nous permet de placer a t'elle corp donner
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
                   // le cell et permet de reuinir la position x et y
                   //Le FondForêt ces les variable dit en haus
                    GridGenerator.AddGridObjectToCell(cell, FondForet, true);  //ce code permet d'ajouter une nouvelle élément dans notre grille
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
