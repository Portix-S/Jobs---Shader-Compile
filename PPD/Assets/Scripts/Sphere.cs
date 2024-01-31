using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
namespace Spheres
{
        [System.Serializable]
        public struct Sphere
        {
            public float yPosition;
            public float speed;
            public float maxY;
            public float minY;
            public int index;
            public int3 id;
            public int3 groupId;
            public int3 groupIdThread;
        }
}
