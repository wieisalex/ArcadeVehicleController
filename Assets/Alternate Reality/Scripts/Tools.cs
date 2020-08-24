using System.Collections.Generic;
using UnityEngine;

namespace AlternateReality
{
    public class Tools
    {
        public static Vector3 GetAverageVector3(List<Vector3> vectors)
        {
            if (vectors.Count == 0)
                return Vector3.zero;

            float x = 0f;
            float y = 0f;
            float z = 0f;

            foreach (Vector3 vector in vectors)
            {
                x += vector.x;
                y += vector.y;
                z += vector.z;
            }

            return new Vector3(x / vectors.Count, y / vectors.Count, z / vectors.Count);
        }
    }
}