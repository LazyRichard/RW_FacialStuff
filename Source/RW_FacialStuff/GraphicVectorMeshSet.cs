﻿namespace FacialStuff
{
    using UnityEngine;

    using Verse;

    public class GraphicVectorMeshSet
    {
        public GraphicMeshSet mesh;

        private Vector2 offSet;

        public GraphicVectorMeshSet(float size, Vector2 offSet)
        {
            this.mesh = new GraphicMeshSet(size);
            this.offSet = offSet;
        }

        public GraphicVectorMeshSet(float sizeX, float sizeY, Vector2 offSet)
        {
            this.mesh = new GraphicMeshSet(sizeX, sizeY);
            this.offSet = offSet;
        }

        public Vector3 OffsetAt(Rot4 rotation)
        {
            switch (rotation.AsInt)
            {
                case 1: return new Vector3(this.offSet.x, 0f, -this.offSet.y);
                case 2: return new Vector3(0f, 0f, -this.offSet.y);
                case 3: return new Vector3(-this.offSet.x, 0f, -this.offSet.y);
                default: return Vector3.zero;
            }
        }

    }
}