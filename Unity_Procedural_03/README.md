# Repos-Proc-dural

//Explication g�n�rale de l'architecture.
D'apres moi la Grid est une structure permettant de g�n�rer a la fois l'espace pour placer nos �l�ments, une �tape pour faire de la procedural.
On peut le d�finir comme cela: 
Grid.Width pour savoir la hauteur, Grid.Lenght pour savoir la largeur d'un RectInt.
Elles est souvant utiliser pour cr�er une sc�ne procedurale.
 for (int x = 0; x < Grid.Width; x++)
        {
            for (int y = 0; y < Grid.Lenght; y++)
            {
            }
        }


Le cell est une unit� �l�mentaire de la grille. elle est utiliser dans cette exemple

                noiseData[x, y] = noise.GetNoise(x, y);
                if (!Grid.TryGetCellByCoordinates(x, y, out var cell)) <----------------
                {
                    Debug.LogError("Grille pas afficher");
                }
                if (noiseData[x, y] < 0.05)
                {
                    //ce code permet de placer a telle grille
                    GridGenerator.AddGridObjectToCell(cell, FondForet, true); // ajoute un �l�ment sur la grid
                }

Le cell est souvant utiliser dans pour cr�er des mondes proc�duraux, mais aussi pour organiser des �l�ments dans une interface utilisateur.




Et la ProceduralGenerationMethod, est a peu pr�s un pr�-code.
Elle est utiles pour g�n�rarer une map procedural. Comme g�n�rer des salle al�atoire pour ensuite les ruinir


Pour ajouter une nouvelle Algorithme architecturale, il faut suivre les �tapes suivantes:
1. [CreateAssetMenu(menuName = "Procedural Generation Method/NOM_de_votre_script_")]  //NOM_de_votre_script_" a la place vous devrier m�ttre votre nom du scripte.
2. Renommer ta heriachie Nom-Script : ProceduralGenerationMethod. comme �a en puis utiliser le parent ProceduralGenerationMethod pour faire des r�f�rences.
3. Dans l'onglet projet vous devrier clique droite> Create Procedural G�n�ration M�thode.
Voila les �tapes a suivre

 public abstract class ProceduralGenerationMethod : ScriptableObject
    {
        [Header("Generation")] 
        [SerializeField] protected int _maxSteps = 1000;

        // ici on ne sauvegarder pas les projet
        [NonSerialized] public ProceduralGridGenerator GridGenerator;
        [NonSerialized] protected RandomService RandomService;
        [NonSerialized] private CancellationTokenSource _cancellationTokenSource;

        protected VTools.Grid.Grid Grid => GridGenerator.Grid;
        







       
       

        protected const string ROOM_TILE_NAME = "Room";
        protected const string CORRIDOR_TILE_NAME = "Corridor";
        protected const string GRASS_TILE_NAME = "Grass";
        protected const string WATER_TILE_NAME = "Water";
        protected const string ROCK_TILE_NAME = "Rock";
        protected const string SAND_TILE_NAME = "Sand";

        // -------------------------------------- BASE ----------------------------------------------------
        




        //celui ci c'est juste pour intialiser 2 valeur
        public void Initialize(ProceduralGridGenerator gridGenerator, RandomService randomService)
        {
            GridGenerator = gridGenerator;
            RandomService = randomService;
        }










//Ce code permet d'�vite que deux g�n�rations s�ex�cutent en m�me temps.
//Annule proprement la pr�c�dente si une nouvelle d�marre.
 public async UniTask Generate()
        {
            // Cancel any ongoing generation
            _cancellationTokenSource?.Cancel();

            // Give it a moment to actually cancel
            await UniTask.Delay(GridGenerator.StepDelay + 100);
            _cancellationTokenSource?.Dispose();

            // Create a new cancellation token for this generation
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // Stay on the main thread to safely manipulate Unity objects
                await UniTask.SwitchToMainThread();
                
                await ApplyGeneration(_cancellationTokenSource.Token);// ce code permet d'arr^ter le temp
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Generation was cancelled.");
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }













//ce bout code si dessou permet de v�rifier si a t'elle la zone cibler est occuper ou non
protected bool CanPlaceRoom(RectInt room, int spacing)
        {
            // optional spacing: extend by one tile around for buffer
            int xMin = Mathf.Max(room.xMin - spacing, 0);//En d�finit le point de d�part ici en x minimum
            int yMin = Mathf.Max(room.yMin - spacing, 0);//En d�finit le point de d�part ici en y minimum
            int xMax = Mathf.Min(room.xMax + spacing, Grid.Width);//En d�finit le point de fin, ici en x max 
            int yMax = Mathf.Min(room.yMax + spacing, Grid.Lenght);//En d�finit le point de fin, ici en y max 

            for (int ix = xMin; ix < xMax; ix++)// il lance �a boucle pour sur chaque ligne
            {
                for (int iy = yMin; iy < yMax; iy++)
                {
                    if (Grid.TryGetCellByCoordinates(ix, iy, out var cell) && cell.ContainObject && cell.GridObject.Template.Name == "Room") 
                        return false;
                }
            }

            return true;
        }


//Ce code si dessous permet d'ajouter un �l�ment sur notre grid, les condition d'activation se trouver dans CanPlaceRoom
 protected void AddTileToCell(Cell cell, string tileName, bool overrideExistingObjects)
        {
            var tileTemplate = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>(tileName);
            GridGenerator.AddGridObjectToCell(cell, tileTemplate, overrideExistingObjects);
        }










