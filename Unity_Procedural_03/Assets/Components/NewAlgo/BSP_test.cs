using System.Threading;
using System.Threading.Tasks;
using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VTools.RandomService;

[CreateAssetMenu(menuName = "Procedurel Generation Method/New algo")]

public class BSP_test : ProceduralGenerationMethod
{
    RectInt TailleRectangulare_Parent;
    RectInt TailleRectangulare_Generation1;
    //BSP_test, BSP_test _childLeaf;
    public BSP_test(int spacing, Node child1, RandomService randomServices, Node node)
    {
    }

    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        for (int i = 0; i < _maxSteps; i++)
        {
            
            cancellationToken.ThrowIfCancellationRequested();
            

            await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
        }

    }
    public class Node
    {
        public Node child1, child2;
        RectInt rectInt;
        private readonly RandomService _randomServices;

        int _spacing = 1;
        int minwidth = 5;
        int maxwidth = 12;
        int minLenght = 5;
        int maxLenght = 12;

        public Node(RandomService randomServices)
        {
            _randomServices = randomServices;
        }
        public void Split()
        {
            //bool horizontal = _randomServices.Chance(probability: 0.5f);
            bool horizontal = true;
            Node firstChild = new Node(_randomServices);
            Node SecondChild = new Node(_randomServices);

            if (horizontal)
            {
                int positionCutY = _randomServices.Range(rectInt.y + rectInt.height / 3 + _spacing, rectInt.height * 2 / 3 + _spacing);
                child1.rectInt = new RectInt(rectInt.x, rectInt.y, rectInt.width, rectInt.height - (rectInt.height - positionCutY));
                child2.rectInt = new RectInt(rectInt.x, rectInt.y+positionCutY, rectInt.width, rectInt.height -positionCutY);
            }
            else
            {
            }
            //_childLeaf.Item1 = new BSP_test(_spacing, child1, _randomServices, this);
            //_childLeaf.Item2 = new BSP_test(_spacing, child2, _randomServices, this);
        }
    }
}
