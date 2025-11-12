using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VTools.Grid;
using VTools.ScriptableObjectDatabase;
using VTools.Utility;

namespace Components.ProceduralGeneration.SimpleRoomPlacement
{

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
}

//Binary tree classement parent est x enfant ou 2 enfantS