using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VTools.Grid;
using VTools.RandomService;
using VTools.ScriptableObjectDatabase;
using VTools.Utility;

[CreateAssetMenu(menuName = "Procedural Generation Method/Dungon Procedural")]// 

public class Dungon_Procedurale : ProceduralGenerationMethod
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<Node_Dungon> _nodes;
    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        // Your procedural generation logic here
        _nodes = new List<Node_Dungon>();
        var allGrid = new RectInt(0, 0, Grid.Width, Grid.Lenght);// ce code permet de définir la taille de la grille
        var Salle = new Node_Dungon(RandomService, this, allGrid);//ce code permet crée la racine de l'arbre
        _nodes.Add(Salle);// en ajoute dans la liste des noeuds
        Salle.ConnectSisters();

    }
}

public class  Node_Dungon
{
    RandomService _randomService;
    Node_Dungon _child1;
    Node_Dungon _child2;

    private readonly BaseGridGenerator _gridGenerator;
    private readonly Dungon_Procedurale _Dungon_Procedurale;

    [SerializeField] public RectInt _Salle;
    public Node_Dungon(RandomService randomService, Dungon_Procedurale bsp2, RectInt Salle)
    {
        _randomService = randomService;
        //_gridGenerator= bsp2.GridGenerator;
        _Dungon_Procedurale = bsp2;
        _Salle = Salle;

    }

    /*private void Split()
    {
        RectInt splitBoundsLeft = default;
        RectInt splitBoundsRight = default;
        bool splitFound = false;

        for (int i = 0; i < _Dungon_Procedurale.MaxSplitAttempt; i++)
        {
            bool horizontal = _randomService.Chance(_Dungon_Procedurale.HorizontalSplitChance);
            float splitRatio = _randomService.Range(_Dungon_Procedurale.SplitRatio.x, _Dungon_Procedurale.SplitRatio.y);

            if (horizontal)
            {
                if (!CanSplitHorizontally(splitRatio, out splitBoundsLeft, out splitBoundsRight))
                {
                    continue;
                }
            }
            else
            {
                if (!CanSplitVertically(splitRatio, out splitBoundsLeft, out splitBoundsRight))
                {
                    continue;
                }
            }

            splitFound = true;
            break;
        }

        // Stop recursion, it's a Leaf !
        if (!splitFound)
        {
            _isLeaf = true;
            PlaceRoom(_Salle);

            return;
        }

        _child1 = new Node_Dungon(_randomService, _gridGenerator, splitBoundsLeft);
        _child2 = new Node_Dungon(_randomService, _gridGenerator, splitBoundsRight);

        _gridGenerator._nodes.Add(_child1);
        _gridGenerator.Tree.Add(_child2);
    }*/







    public void ConnectSisters()
    {
        // It's a leaf, nothing to do here.
        if (_child1 == null || _child2 == null)
            return;

        // Connect sisters
        ConnectNodes(_child1, _child2);

        // Connect child of sisters
        _child1.ConnectSisters();
        _child2.ConnectSisters();
    }
    private void ConnectNodes(Node_Dungon node1, Node_Dungon node2)
    {
        var center1 = node1.GetLastChild()._Salle.GetCenter();
        var center2 = node2.GetLastChild()._Salle.GetCenter();

        CreateDogLegCorridor(center1, center2);
    }
    private Node_Dungon GetLastChild()
    {
        if (_child1 != null)
        {
            return _child1.GetLastChild();
        }

        return this;
    }
    private void CreateDogLegCorridor(Vector2Int start, Vector2Int end)
    {
        bool horizontalFirst = _randomService.Chance(0.5f);

        if (horizontalFirst)
        {
            // Draw horizontal line first, then vertical
            //CreateHorizontalCorridor(start.x, end.x,start.y);
            //CreateVerticalCorridor(start.y, end.y, end.x);
            Create2_verticaux_Horizon_Corridor(start.x, end.x, 0,start.y);
            Create2_verticaux_Horizon_Corridor(start.y, end.y, end.x,0);
        }
        else
        {
            // Draw vertical line first, then horizontal
            //CreateVerticalCorridor(start.y, end.y, start.x);
            //CreateHorizontalCorridor(start.x, end.x, end.y);
            Create2_verticaux_Horizon_Corridor(start.y, end.y, start.x,0);
            Create2_verticaux_Horizon_Corridor(start.x, end.x, 0, end.y);
            
        }
    }
 /*   private void CreateHorizontalCorridor(int x1, int x2, int y)
    {
        int xMin = Mathf.Min(x1, x2);
        int xMax = Mathf.Max(x1, x2);

        for (int x = xMin; x <= xMax; x++)
        {
            if (!_gridGenerator.Grid.TryGetCellByCoordinates(x, y, out var cell))
                continue;

            var groundTemplate = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Corridor");
            _gridGenerator.AddGridObjectToCell(cell, groundTemplate, false);
        }
    }

    /// Creates a vertical corridor from y1 to y2 at the given x coordinate
    private void CreateVerticalCorridor(int y1, int y2, int x)
    {
        int yMin = Mathf.Min(y1, y2);
        int yMax = Mathf.Max(y1, y2);

        for (int y = yMin; y <= yMax; y++)
        {
            if (!_gridGenerator.Grid.TryGetCellByCoordinates(x, y, out var cell))
                continue;

            var groundTemplate = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Corridor");
            _gridGenerator.AddGridObjectToCell(cell, groundTemplate, false);
        }
    }*/

    private void Create2_verticaux_Horizon_Corridor(int i1, int i2, int X_carret, int Y_carret)
    {
        int iMin = Mathf.Min(i1, i2);
        int iMax = Mathf.Max(i1, i2);

        if(Y_carret>0)
        {
            for (int x = iMin; x <= iMax; x++)
            {
                if (!_gridGenerator.Grid.TryGetCellByCoordinates(x, Y_carret, out var cell)) // la var cell est utiliser
                    continue;

                var groundTemplate = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Corridor");
                _gridGenerator.AddGridObjectToCell(cell, groundTemplate, false);
            }
        }
        else if (X_carret > 0)
        {
            for (int y = iMin; y <= iMax; y++)
            {
                if (!_gridGenerator.Grid.TryGetCellByCoordinates(X_carret, y, out var cell))
                    continue;

                var groundTemplate = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Corridor");
                _gridGenerator.AddGridObjectToCell(cell, groundTemplate, false);
            }
        }
        else
        {
            Debug.Log("erreur de création de couloir");
        }
        
    }
}
