using UnityEngine;

namespace HayWay.Runtime.Components
{
    public class StagePart : PooleabeObject
    {
        public float size = 6;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            float cellSize = 1;
            float width = size;
            float height = size;

            float gridWidth = width * cellSize;
            float gridHeight = height * cellSize;

            Vector3 origin = transform.position - new Vector3(gridWidth / 2, 0, gridHeight / 2);

            for (int x = 0; x <= width; x++)
            {
                Gizmos.DrawLine(
                    origin + new Vector3(x * cellSize, 0, 0),
                    origin + new Vector3(x * cellSize, 0, gridHeight)
                );
            }

            for (int y = 0; y <= height; y++)
            {
                Gizmos.DrawLine(
                    origin + new Vector3(0, 0, y * cellSize),
                    origin + new Vector3(gridWidth, 0, y * cellSize)
                );
            }
        }
    }

}