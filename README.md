#Étude Procedural Noise: Permet de créer une map aléatoire a partie d'un song
Noise_Automata.cs:

//On début le program script en Créer notre AssetMenue, il est important de le renommer avec votre le nom de votre script, exemple "Noise_Automata".
//Ce programme permet de créer est d'utiliser votre propre code ProceduralGenerationMethod, sans ça vous ne pourrier pas ajouter dans vos fiche document. 

[CreateAssetMenu(menuName = "Procedural Generation Method/Noise_Automata")]        

public class Noise_Procedural : ProceduralGenerationMethod  <---  /Attention, pour la procedure, vous dever remplacer MonoBehaviour par ProceduralGenerationMethod. Sinon on ne pourrat pas utiliser les précode dans ProceduralGenerationMethod.cs
{
    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)  <-- ce code est prédéfinit sur tout les enfant de ProceduralGenerationMethod. elle fonction comme un void Start()
    {
        //Pour pouvoir séléctionner et placer une préfable a sur ma grille, j'utiliser un GetScriptableObject dans le script:ScriptableObjectDatabase pour l'enregistrer en variable
        //Bien sur, on noublie pas de renommer entre les bordure le NOM de votre Asset Possédant sur lui le script "GridObjectController.cs"
        var FondGrass = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Grass");
        var FondSable = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Sand");
        var FondEau  = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Water");
        var FondForet  = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Foret");
        ...                                                                                                     <- Vous en fait pas,il n'y a rien ici
        int Scale_Map = Random.Range(1234, 3000);              // j'ai définit une int pour appliquer une valeur aléatoire, elle aurat pour objectif de changer la seed de notre FastNoiseLite ou plutot notre song numérique. 
        ...
        FastNoiseLite noise = new FastNoiseLite(Scale_Map);   //Ce code permet de créer un nouveau song. Vous remarquer que mon petit code random est inscrit 
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);   //Dans ce code en change un enumerate NoiseType dans le FastNoiseLite
        ...
        //Le Grid.width permet compter la Largeur d'une grille.
        //Le Grid.Lenght c'est la taille max d'une grille
        float[,] noiseData = new float[Grid.Width, Grid.Lenght];    //En créer une nouvelle floatla longueur et largeur de la grille dans noisedata
        ...
        ...
        for (int x = 0; x < Grid.Width; x++)   //Pour cette boucle en utiliser le Grid.width pour gérer la largeur de la Grille.
        {                                      //Je tient a noter que la Grid, est une valeur inscrite dans le script GridObjectController
            for (int y = 0; y < Grid.Lenght; y++)       //Pareille mais le Grid.lenght, est utiliser pour la longueur. ""
            {
                noiseData[x, y] = noise.GetNoise(x, y);         //En initialiser notre noiseData
                if (!Grid.TryGetCellByCoordinates(x, y, out var cell)) //c'est une vêrification si la !Grid.TryGetCellByCoordinates et en définit le resultat var cell
                {
                    Debug.LogError("Grille pas afficher");
                }
                if (noiseData[x, y] < 0.05) <------------//En fonction de la tensiter du song, en peut
                {
                    GridGenerator.AddGridObjectToCell(cell, FondForet, true); <-------//ce code permet d'ajouter une nouvelle élément dans notre grille + Le FondForêt ces les variable dit en haus
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
    } <----------------------------<---------------------//Maintenant il faudrat créer un une cellule Automata: 
}-----------------------------------------------------//Document>Projet> click droite> créated> Procedural Generation Method> select le nom de votre script
..
...
..
---------------------------------------------------------------------------------------------------------------------------------------------------------------------
..
...
..


















