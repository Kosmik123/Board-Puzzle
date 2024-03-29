using UnityEngine;

namespace Bipolar.PuzzleBoard.Rectangular
{
    public interface IRectangularBoard : IBoard
    {
        Vector2Int Dimensions { get; }
    }

    public class RectangularBoard : Board<RectangularBoardData>, IRectangularBoard
    {
        public event System.Action<Vector2Int> OnDimensionsChanged;

        [SerializeField]
        private Vector2Int dimensions;
        public Vector2Int Dimensions
        {
            get => dimensions;
            set
            {
                dimensions = new Vector2Int(Mathf.Max(1, value.x), Mathf.Max(1, value.y));
                CalculateOtherDimensions();
                OnDimensionsChanged?.Invoke(dimensions);
            }
        }

        private Vector3 localCenter;

        protected override void Awake()
        {
            base.Awake();
            CalculateOtherDimensions();
        }

        protected override void CreateBoardData()
        {
            boardData = new RectangularBoardData(dimensions.x, dimensions.y, Grid.cellLayout);
        }

        private void CalculateOtherDimensions()
        {
            Vector3Int lastCellCoord = (Vector3Int)Dimensions - Vector3Int.one;
            var topRight = Grid.CellToLocal(lastCellCoord);
            if (Grid.cellLayout == GridLayout.CellLayout.Hexagon && ((Vector3Int)Dimensions - Vector3Int.one).y % 2 == 0)
            {
                var right = Grid.Swizzle(Grid.cellSwizzle, Vector3.right);
                var upForward = Grid.Swizzle(Grid.cellSwizzle, new Vector3(0, 1, 1));
                topRight = Vector3.Scale(upForward, topRight);
                topRight += Vector3.Scale(right, Grid.CellToLocalInterpolated(lastCellCoord + Vector3.one / 2));
            }
            localCenter = Vector3.Scale(topRight / 2, transform.lossyScale); 
        }

        public override Vector3 CoordToWorld(Vector2 coord) => base.CoordToWorld(coord) - localCenter;
        public override Vector2Int WorldToCoord(Vector3 worldPosition) => base.WorldToCoord(worldPosition + localCenter);

        private void OnValidate()
        {
            Dimensions = dimensions;
        }

        private void OnDrawGizmosSelected()
        {
            var darkColor = Color.black;
            var lightColor = Color.white;
            var redColor = Color.red;
            lightColor.a = darkColor.a = redColor.a = 0.3f;

            for (int j = 0; j < dimensions.y; j++)
            {
                for (int i = 0; i < dimensions.x; i++)
                {
                    Vector3 position = CoordToWorld(i, j);
                    Vector3 cubeSize = Vector3.Scale(Grid.cellSize, transform.lossyScale);
                    bool isEven = (i + j) % 2  == 0;
                    switch (Grid.cellLayout)
                    {
                        case GridLayout.CellLayout.Rectangle:
                            Gizmos.color = isEven ? darkColor : lightColor;
                            Gizmos.DrawCube(position, cubeSize);
                            break;

                        case GridLayout.CellLayout.Hexagon:
                            int remainder = (i - (j % 2)) % 3;
                            Gizmos.color = remainder == 0 ? darkColor : remainder == 1 ? lightColor : redColor;
                            Gizmos.DrawSphere(position, cubeSize.z / 2);
                            break;

                        default:
                            Gizmos.color = isEven ? darkColor : lightColor;
                            var matrix = Gizmos.matrix;
                            var isometricRotation = Quaternion.AngleAxis(45, Vector3.forward);
                            Gizmos.matrix = Matrix4x4.TRS(position, isometricRotation, cubeSize / 2);
                            Gizmos.DrawCube(Vector3.zero, Vector3.one);
                            Gizmos.matrix = matrix;
                            break;
                    }
                }
            }
        }
    }
}
