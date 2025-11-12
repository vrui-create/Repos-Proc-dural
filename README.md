#Étude Procedural Noise: Permet de créer une map aléatoire a partie d'un song
PROCEDURAL: Noise_Automata.cs:

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
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
..

PROCEDURAL BSP
BSP Procedural, controller l'aléatoire en utilisant la division:

using System.Collections.Generic;
using System.Threading;
using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VTools.Grid;
using VTools.RandomService;
using VTools.ScriptableObjectDatabase;
using VTools.Utility;

// ce code permet de créer un menu déroulant dans unity pour choisir le type de génération procédurale
[CreateAssetMenu(menuName = "Procedural Generation Method/BSP 2")]
// au lieu de hériter de monobehaviour on hérite de ProceduralGenerationMethod, elle nous permet d'accéder à la grille et au générateur de grille
public class BSP2 : ProceduralGenerationMethod
{

    [Header("Split Parameters")]
    [Range(0,1)] public float HorizontalSplitChance = 0.5f;
    public Vector2 SplitRatio = new(0.3f, 0.7f);
    public int MaxSplitAttempt = 5;
    
    [Header("Leafs Parameters")]
    public Vector2Int LeafMinSize = new(8, 8);
    public Vector2Int RoomMaxSize = new(7, 7);
    public Vector2Int RoomMinSize = new(5, 5);
    
    [Header("Debug")]
    public List<Node> Tree; // On définir une liste de class "Node". elle aurat pour objectif de créer un pont a tous les enfants, et ainsi une route ou interseption entre salle
    public ProceduralGridGenerator BSP2_ProceduralGridGenerator; // Ce code permet de réparer mon probleme de connection 



    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken) //Pour celui ci, il est utiliser comme un void Start(), et a créer pour tous enfant de ProceduralGenerationMethod
    {
        //En utiliser protected override async UniTask ApplyGeneration(CancellationToken cancellationToken) permetant d'appliquer la génération procédurale
        BSP2_ProceduralGridGenerator = GridGenerator; // Ce code me permet de l'utiliser dans la class node  // sa régle une erreur -----> _gridGenerator = bsp2.BSP2_ProceduralGridGenerator;

        Tree = new List<Node>(); // En créer une nouvelle list class Node
        
        var allGrid = new RectInt(0, 0, Grid.Width, Grid.Lenght); //Ce code permet de définir notre grille principale
        // Créer tout l'arbre.
        // Le root, en définit comme la base de notre hérachie.
        var root = new Node(RandomService, this, allGrid);
        Tree.Add(root);//En ajoute dans la liste Node

        root.ConnectSisters(); // comme son nom l'indique la on connecter les 2enfant exemple A1 et B1
    }
}

[System.Serializable]
public class Node
{
    [SerializeField] private RectInt _room;
    private readonly RandomService _randomService;
    private readonly BaseGridGenerator _gridGenerator;
    private readonly BSP2 _bsp2;

    bool _isLeaf;

    //en créé 2 child, pour faire une procedure controller.
    //L'objectif est de couper de façon random la longueur ou largeur de notre grille parentale est assigner son rectint pour le _child 1 et 2
    private Node _child1; 
    private Node _child2;

    public Node(RandomService randomService, BSP2 bsp2, RectInt room) //Initialisation tous les infos de la hérachie 
    {
        _randomService = randomService;
        _bsp2 = bsp2;
        _gridGenerator = bsp2.BSP2_ProceduralGridGenerator;
        _room = room;
        Split();
    }

    private void Split()
    {
        //on utiliser les variable: splitBoundsRight et splitBoundsLeft  pour attribuer les nouveaux enfants de Node
        RectInt splitBoundsLeft = default; 
        RectInt splitBoundsRight = default;
        bool splitFound = false; 

        for (int i = 0; i < _bsp2.MaxSplitAttempt; i++)//Le MaxSplitAttempt de _bsp2, c'est le nombre de fois ou on le répéter
        {
            bool horizontal = _randomService.Chance(_bsp2.HorizontalSplitChance);// on initaliser le boolean horizontal avec random, il choisiras entre deux option true ou false
            float splitRatio = _randomService.Range(_bsp2.SplitRatio.x, _bsp2.SplitRatio.y);
        
            if (horizontal)// Onfonction du resultat de horizontal, il détermineras si la coupure se feras sur les coters ou sur la hauteur.
            {
                if (!CanSplitHorizontally(splitRatio, out splitBoundsLeft, out splitBoundsRight))// Grace a la fonction CanSplitHorizontally, nous pouvant savoir si on peut rajouter des élément a l'interieur
                {
                    continue;
                }
            }
            else
            {
                if (!CanSplitVertically(splitRatio, out splitBoundsLeft, out splitBoundsRight))// On appelle la fonction CanSplitVertically, afin de savoir si il y a de la place dans ce petit rectangle.
                {
                    continue;
                }
            }
            splitFound = true;
            break;
        }

        
        if (!splitFound)// condition permetant de savoir si oui ou non en place la salle
        {
            _isLeaf = true;
            PlaceRoom(_room);
            return;
        }
        // C'est 2 code perme créer deux nouvelles salle avec leur propre code node.
        _child1 = new Node(_randomService, _bsp2, splitBoundsLeft); 
        _child2 = new Node(_randomService, _bsp2, splitBoundsRight);
        // aprés avoir créer nos 2 node"enfant" on doit les noter dans leur branche respectible, un petit exemple ne feras pas de mal:
        _bsp2.Tree.Add(_child1);
        _bsp2.Tree.Add(_child2);
    }

    private bool CanSplitHorizontally(float splitRatio ,out RectInt firstSplit, out RectInt secondSplit)
    {
        //widthSplit: qui en français veut dire Largeur de la division de la salle
        //Pour ce code on souhaite savoir sur la largeur ou se feras sa premier division.
        int widthSplit = Mathf.RoundToInt(_room.width * splitRatio);

        var firstSplitWidth = widthSplit; //initialisation de la premier division sur la hauteur de la salle
        var firstSplitHeight = _room.height; // pareil pour ce code en veut savoir ou se feras sa premier division sur la hauteur de la salle
        firstSplit = new RectInt(_room.xMin, _room.yMin, firstSplitWidth, firstSplitHeight);//Pour créer notre premier coupur, nous utilisant room
        
        var secondSplitWidth = _room.width - widthSplit; //initialisation de la premier division sur la hauteur de la salle
        var secondSplitHeight = _room.height;// pareil pour ce code en veut savoir ou se feras sa premier division sur la hauteur de la salle
        secondSplit = new RectInt(_room.xMin + widthSplit, _room.yMin, secondSplitWidth, secondSplitHeight);//Pour créer notre deuxiéme coupur, nous utilisant room


        // pour c'est 2 condition en vêrifie si le firstSplit ne dépasse pas les _bsp2.LeafMinSize: x et y 
        if (firstSplit.width < _bsp2.LeafMinSize.x || firstSplit.height < _bsp2.LeafMinSize.y)
        {
            return false;
        }
        // pareil que le commentaire précéfent, mais qu'a la différence que c'est pour la seconde division 
        if (secondSplit.width < _bsp2.LeafMinSize.x || secondSplit.height < _bsp2.LeafMinSize.y)
        {
            return false;
        }
        
        return true;
    }
    
    private bool CanSplitVertically(float splitRatio ,out RectInt firstSplit, out RectInt secondSplit)// dans cette fonction on vêrifie si il y a déja une construction a telle position
    {
        //widthSplit: qui en français veut dire Largeur de la division de la salle
        //Pour ce code on souhaite savoir sur la largeur ou se feras sa premier division.
        int heightSplit = Mathf.RoundToInt(_room.height * splitRatio); //initialisation de la Hauteur de séparation, tout en arrondisant le resultat

        var firstSplitWidth = _room.width; //initialisation de la premier division sur la hauteur de la salle
        var firstSplitHeight = heightSplit; // pareil pour ce code en veut savoir ou se feras sa premier division sur la hauteur de la salle
        firstSplit = new RectInt(_room.xMin, _room.yMin, firstSplitWidth, firstSplitHeight);//Pour créer notre premier coupur, nous utilisant room

        var secondSplitWidth = _room.width;//initialisation de la premier division sur la hauteur de la salle
        var secondSplitHeight = _room.height - heightSplit;// pareil pour ce code en veut savoir ou se feras sa premier division sur la hauteur de la salle
        secondSplit = new RectInt(_room.xMin, _room.yMin + heightSplit, secondSplitWidth, secondSplitHeight);//Pour créer notre deuxiéme coupur, nous utilisant room
        // pour c'est 2 condition en vêrifie si le firstSplit ne dépasse pas les _bsp2.LeafMinSize: x et y 
        if (firstSplit.width < _bsp2.LeafMinSize.x || firstSplit.height < _bsp2.LeafMinSize.y)
        {
            return false;
        }
        // pareil que le commentaire précéfent, mais qu'a la différence que c'est pour la seconde division 
        if (secondSplit.width < _bsp2.LeafMinSize.x || secondSplit.height < _bsp2.LeafMinSize.y)
        {
            return false;
        }
        
        return true;
    }
    
    // dans cette fonction permet de placer la salle.
    private void PlaceRoom(RectInt room)
    {
        //pour les deux variable assigne un aléatoire x et y  de notre _bsp2.RoomMinSize
        var newRoomLength = _randomService.Range(_bsp2.RoomMinSize.x, _bsp2.RoomMaxSize.x + 1);
        var newRoomWidth = _randomService.Range(_bsp2.RoomMinSize.y, _bsp2.RoomMaxSize.y + 1);


        //initialisation des variable: newRoomLength et newRoomWidth
        room.width = newRoomWidth;
        room.height = newRoomLength;
        
        // Re-affecter la room ou cas ou 
        _room = room;
        

        //Résumer du code si dessous: en utiliser un for pour vêrifier tous la longueur et hauteur de notre scéne
        
        for (int ix = room.xMin; ix < room.xMax; ix++)
        {
            for (int iy = room.yMin; iy < room.yMax; iy++)
            {
                if (!_gridGenerator.Grid.TryGetCellByCoordinates(ix, iy, out var cell)) //et la condition, elle permet de vêrifier si a tels emplacement il y a déja une salle
                    continue;// le cell est un resultat en format variable
                    
                var groundTemplate = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Room"); // on assigne une variable groundTemplate pour pouvoir l'invoquer.
                _gridGenerator.AddGridObjectToCell(cell, groundTemplate, true);//ce code permet d'invoquer sur la grille un objet "Room"
            }
        }
    }

    private Node GetLastChild()// cette fonction permet de savoir si il reste des enfants. Le return this; va retourner le resultat.
    {
        if (_child1 != null)
        {
            return _child1.GetLastChild();
        }
        return this;
    }

    public void ConnectSisters()
    {
        // Permet de vêrifier les deux enfants existe ou non, c'est une sécuriter.
        if (_child1 == null || _child2 == null) 
            return;
        
      
        ConnectNodes(_child1, _child2);  //on appelle cette fonction pour connecter nos deux soeurs entre elles.
                                         //traduction: connecter 2 salle jumelle entre elle avec une route
        _child1.ConnectSisters();
        _child2.ConnectSisters();
    }

    private void ConnectNodes(Node node1, Node node2)//Pour ce code en veut savoir si nos 2 child on un enfant
    {
        var center1 = node1.GetLastChild()._room.GetCenter();
        var center2 = node2.GetLastChild()._room.GetCenter();
        
        CreateDogLegCorridor(center1, center2);// une fois quand a confirmer le centre de nos 2 parents, c'est a dire centre1 de "A1,A2" et centre2 "B1,B2" en fais passer l'infos a la fonction CreateDogLegCorridor
    }
    /// Crée un couloir en forme de L entre deux points, en choisissant aléatoirement l'ordre horizontal ou vertical.
    private void CreateDogLegCorridor(Vector2Int start, Vector2Int end)// Ce code utiliser nos 2 Vecteur:[centre1= "A1,A2" et centre1= "B1,B2"]
    {
        bool horizontalFirst = _randomService.Chance(0.5f);// Dans ce code on laisse le hazard choisir entre tracer une ligne horizontale ou verticale.

        if (horizontalFirst)
        {
            // ces bout code permet de Tracez d'abord une ligne horizontale, puis une ligne verticale.
            CreateHorizontalCorridor(start.x, end.x, start.y); //le int start.x, c'est tout simplement le Vector2Int Centre1.x est le int end.x 
            CreateVerticalCorridor(start.y, end.y, end.x); // l'autre c'est la même explication quand haut, mais a la différence que la fonction et l'axe y qui a était donner
        }
        else
        {
            // ces bout code permet de Tracez en premier une ligne verticale, puis une ligne horizontale.
            CreateVerticalCorridor(start.y, end.y, start.x);
            CreateHorizontalCorridor(start.x, end.x, end.y);
        }
    }

    // D'apres la traduction google trad: <<Crée un couloir horizontal de x1 à x2 à la coordonnée y donnée.>> On oublie pas que l'axe y nous permet de tracer sur les bonne cordonner.
    private void CreateHorizontalCorridor(int x1, int x2, int y) // Dans ce code, j'ai l'impréssion que je suis devenue un pnj: d'apres les renseignement laisser par CreateDogLegCorridor elle vous a transmi lordre de passage
    {
        //A noter, pour se qui sont perdu, le centre1 c'est la distance moyenne entre nos 2 enfant node.
        //On Résumer voyer comme un fil invisible qui relier nos 2 enfants est que le milieu entre nos 2enfant devient le centre1
        //...
        //On initialiser le xMin et xMax de nos 2 enfant node sur l'axe x
        int xMin = Mathf.Min(x1, x2);
        int xMax = Mathf.Max(x1, x2);

        for (int x = xMin; x <= xMax; x++)//Grâce au cordonner fournit nous pourront placer notre Corridor = "route" est l'axe y nous permet de controller la hauteur de notre route
        {
            if (!_gridGenerator.Grid.TryGetCellByCoordinates(x, y, out var cell))//si les 3 conditions sont remplir, le resultat x, y seras transformer on variable cell.
                continue;
            
            var groundTemplate = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Corridor"); // <-------- //J'ai utiliser c'est variables pour définir les différents types de terrain que je vais utiliser pour générer la map
            _gridGenerator.AddGridObjectToCell(cell, groundTemplate, false);// <-------- //ce code permet d'ajouter une nouvelle élément dans notre grille
        }
    }

    // Crée un couloir vertical de y1 à y2 à la coordonnée x donnée.
    private void CreateVerticalCorridor(int y1, int y2, int x)//
    {
        //A noter, pour se qui sont perdu, le centre1 c'est la distance moyenne entre nos 2 enfant node.
        //On Résumer voyer comme un fil invisible qui relier nos 2 enfants est que le milieu entre nos 2enfant devient le centre1
        //...
        //On initialiser le yMin et yMax de nos 2 enfant node sur l'axe y
        int yMin = Mathf.Min(y1, y2);
        int yMax = Mathf.Max(y1, y2);
        
        for (int y = yMin; y <= yMax; y++) 
        {
            if (!_gridGenerator.Grid.TryGetCellByCoordinates(x, y, out var cell))//Pareille pour ceux code: si les 3 conditions sont remplir, le resultat x, y seras transformer on variable cell.
                continue;
                
            var groundTemplate = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Corridor"); // <-------- //J'ai utiliser c'est variables pour définir les différents types de terrain que je vais utiliser pour générer la map
            _gridGenerator.AddGridObjectToCell(cell, groundTemplate, false);// <-------- //ce code permet d'ajouter une nouvelle élément dans notre grille
        }
    }
}
...
..
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
..
...

PROCEDURAL SimpleRoomPlacement


// ce code permet de créer un menu déroulant dans unity pour choisir le type de génération procédurale
[CreateAssetMenu(menuName = "Procedural Generation Method/Simple Room Placement")]
// au lieu de hériter de monobehaviour on hérite de ProceduralGenerationMethod, elle nous permet d'accéder à la grille et au générateur de grille

public class SimpleRoomPlacement : ProceduralGenerationMethod
{
    [Header("Room Parameters")]
    //Ce code SerializeField permet d'enregistrer les valeur indiquer
    [SerializeField] private int _maxRooms = 8; //En définit une limite de salle a générer, de base elle est de 10, je préfere qu'il on génére moin
    //Pour les 2 Vector2Int "_roomMinSize et _roomMaxSize" vont être créer comme nouvelle taille min et max.
    [SerializeField] private Vector2Int _roomMinSize = new(5, 5);  
    [SerializeField] private Vector2Int _roomMaxSize = new(12, 8);

    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        List<RectInt> placedRooms = new(); //Création d'une nouvelle liste <RectInt> placedRooms
        int roomsPlacedCount = 0;
        int attempts = 0;

        for (int i = 0; i < _maxSteps; i++)
        {
            // Check for cancellation
            cancellationToken.ThrowIfCancellationRequested();

            if (roomsPlacedCount >= _maxRooms)
            {
                break;
            }

            attempts++;

            // Avec les valeur "_roomMinSize et _roomMaxSize" en déside de les utiliser pour faire un random de taille.
            int width = RandomService.Range(_roomMinSize.x, _roomMaxSize.x + 1);// <----- pour ce code, elle se porteras sur la largeur de la salle
            int lenght = RandomService.Range(_roomMinSize.y, _roomMaxSize.y + 1);// <----- pour ce code, elle se porteras sur la hauteur de la salle

            // choose random position so entire room fits into grid
            //On créer 2 variable x et y nous permettant d'initialiser le random longueur pour le x et largeur pour le y
            int x = RandomService.Range(0, Grid.Width - width);
            int y = RandomService.Range(0, Grid.Lenght - lenght);

            // dans celui ci on créer une nouvelle Variable RectInt appellon newRoom
            RectInt newRoom = new RectInt(x, y, width, lenght); //Et on assigne le "x, y" pour la position minimume, et le "width, lenght" pour définir la longueur et la largeur

            // Le CanPlaceRoom est une préte code, c'est a dire que j'utiliser le code d'un autre dev
            //Et pour sa fonction, on le demander si a telle position es que la nouvelle salle se superpose, je supposer que ka fonction s'arrêter
            //Enfin tant que le que le nombre de salle n'est pas remplir, ce programme va continuer a placer de façon random une nouvelle salle
            if (!CanPlaceRoom(newRoom, 1))
                continue;// je suis confus sur l'utiliter de continue, es que c'est un filtre ou a sa fonction quand il détécte une anomalie

            PlaceRoom(newRoom);// ce code permet de placer une nouvelle salle sur la grille
            placedRooms.Add(newRoom);// ce code ajoute dans la liste placedRooms

            roomsPlacedCount++;// Pour êviter que ce programme se répéter a l'infinie, en fait un systeme simple pour changer la valeur roomsPlacedCount, et ainsi de limiter la boucle programmation

            await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);// ce code est génial, elle permet de stopper temporairement l'UniTask
        }

        if (roomsPlacedCount < _maxRooms)
        {
            Debug.LogWarning($"RoomPlacer Only placed {roomsPlacedCount}/{_maxRooms} rooms after {attempts} attempts.");
        }

        if (placedRooms.Count < 2)
        {
            Debug.Log("Not enough rooms to connect.");
            return;
        }

        // CORRIDOR CREATIONS
        for (int i = 0; i < placedRooms.Count - 1; i++)// vêrification si on a terminer tous la liste
        {
            // ce code en dessous permet de Vérifier l'annulation
            cancellationToken.ThrowIfCancellationRequested();
            //Grâce a la liste placedRooms<RectInt> | elle nous permet de ranger on ordre croissant
            //Par la suite on va les utiliser pour créer des pont ou des outes qui vont se connécter au autre salle

            //ATTENTION a prendre en compte que le i est définit dans le for (int i = 0; i < placedRooms.Count - 1; i++)

            Vector2Int start = placedRooms[i].GetCenter(); //en initialiser le VectorInt start comme point de départ dans notre liste placedRooms[i]
            Vector2Int end = placedRooms[i + 1].GetCenter(); //en initialiser le VectorInt end comme point de départ dans notre liste placedRooms[i + 1]

            CreateDogLegCorridor(start, end); //ce code nous permet de créer un pont entre 2 variable Vector2Int: start et end

            await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);//Pour êviter que nos PC ne se crache pas, en fait un temps de pause, pour chaque fin de boucle
        }

        BuildGround();
    }

    // -------------------------------------- ROOM ---------------------------------------------

    /// Marks the grid cells of the room as occupied
    private void PlaceRoom(RectInt room) // Grace a la variable RectInt room, en pourrat prend les infos position x, y pui la hauteur et largeur de la salle
    {
        // Pour que la boucle for ne fonctionne, en initialiser le ix par la position minimume de room sur l'axe xMin, en vêrifie si elle est bien inférieur sa position max
        for (int ix = room.xMin; ix < room.xMax; ix++)
        {
            //Pareille pour iy.
            for (int iy = room.yMin; iy < room.yMax; iy++)
            {
                // ce code est une condition, elle vêrifie sur telle position ix et iy si un objet est déja dessus
                //si a telle position il n'y a pas d'objet en initialiser le resultat ix et iy dans la variable cell
                if (!Grid.TryGetCellByCoordinates(ix, iy, out var cell))
                    continue;

                AddTileToCell(cell, ROOM_TILE_NAME, true);// ce code permet d'installer un élément dans ma grille
            }
        }
    }

    // -------------------------------------- CORRIDOR --------------------------------------------- 

    // cette fonction auras pour fonction de créer un pond entre nos vecteur
    // le start et end sont déja définit en haut. On cas ou vous navez pas suivit
    private void CreateDogLegCorridor(Vector2Int start, Vector2Int end) 
    { 
        bool horizontalFirst = RandomService.Chance(0.5f);// cette variable horizontalFirst sera utiliser pour choisir l'ordre d'emplacement deux vectorint 

        if (horizontalFirst)
        {
            // ces bout code permet de Tracez d'abord une ligne horizontale, puis une ligne verticale.
            CreateHorizontalCorridor(start.x, end.x, start.y);//le int start.x, c'est tout simplement le Vector2Int Centre1.x est le int end.x 
            CreateVerticalCorridor(start.y, end.y, end.x);// l'autre c'est la même explication quand haut, mais a la différence que la fonction et l'axe y qui a était donner
        }
        else
        {
            // ces bout code permet de Tracez en premier une ligne verticale, puis une ligne horizontale.
            CreateVerticalCorridor(start.y, end.y, start.x);// l'autre c'est la même explication quand haut, mais a la différence que la fonction et l'axe y qui a était donner
            CreateHorizontalCorridor(start.x, end.x, end.y);//le int start.x, c'est tout simplement le Vector2Int Centre1.x est le int end.x 
        }
    }

    /// Creates a horizontal corridor from x1 to x2 at the given y coordinate
    private void CreateHorizontalCorridor(int x1, int x2, int y)
    {
        //A noter, pour se qui sont perdu, le centre1 c'est la distance moyenne entre nos 2 enfant node.
        //On Résumer voyer comme un fil invisible qui relier nos 2 enfants est que le milieu entre nos 2enfant devient le centre1
        //...
        //On initialiser le xMin et xMax de nos 2 enfant node sur l'axe x
        int xMin = Mathf.Min(x1, x2);
        int xMax = Mathf.Max(x1, x2);

        for (int x = xMin; x <= xMax; x++)//Grâce au cordonner fournit nous pourront placer notre Corridor = "route" est l'axe y nous permet de controller la hauteur de notre route
        {
            if (!Grid.TryGetCellByCoordinates(x, y, out var cell))//si les 3 conditions sont remplir, le resultat "x, y" seras transformer on variable cell.
                continue;

            AddTileToCell(cell, CORRIDOR_TILE_NAME, true);// <-------- //ce code permet d'ajouter une nouvelle élément dans notre grille
        }
    }

    /// Creates a vertical corridor from y1 to y2 at the given x coordinate
    private void CreateVerticalCorridor(int y1, int y2, int x)
    {
        //A noter, pour se qui sont perdu, le centre1 c'est la distance moyenne entre nos 2 enfant node.
        //On Résumer voyer comme un fil invisible qui relier nos 2 enfants est que le milieu entre nos 2enfant devient le centre1
        //...
        //On initialiser le xMin et xMax de nos 2 enfant node sur l'axe x
        int yMin = Mathf.Min(y1, y2);
        int yMax = Mathf.Max(y1, y2);

        for (int y = yMin; y <= yMax; y++)//Grâce au cordonner fournit nous pourront placer notre Corridor = "route" est l'axe y nous permet de controller la hauteur de notre route
        {
            if (!Grid.TryGetCellByCoordinates(x, y, out var cell))//si les 3 conditions sont remplir, le resultat "x, y" seras transformer on variable cell.
                continue;

            AddTileToCell(cell, CORRIDOR_TILE_NAME, true);// <-------- //ce code permet d'ajouter une nouvelle élément dans notre grille
        }
    }

    // -------------------------------------- GROUND --------------------------------------------- 

    private void BuildGround()
    {
        var groundTemplate = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Grass"); // <-------- //cette variables seras utiliser pour mêttre de l'herbe ou terre en arrier plan

        
        for (int x = 0; x < Grid.Width; x++)// en initialiser le x a 0, si x est toujour inférieur a la hauteur de la grille, la boucle se répéteras indéfinément
        {
            for (int z = 0; z < Grid.Lenght; z++)// en initialiser le z a 0, si z est toujour inférieur a la largeur de la grille, la boucle se répéteras indéfinément
            {
                if (!Grid.TryGetCellByCoordinates(x, z, out var chosenCell))//si les 3 conditions sont remplir, le resultat x, y seras transformer on variable chosenCell.
                {
                    Debug.LogError($"Unable to get cell on coordinates : ({x}, {z})");
                    continue;
                }
                //ce code nous permet d'ajouter un élément sur notre scéne
                //
                GridGenerator.AddGridObjectToCell(chosenCell, groundTemplate, false);// <-------- //AddGridObjectToCell: ce code permet d'ajouter une nouvelle élément dans notre grille
            }
        }
    }
}
