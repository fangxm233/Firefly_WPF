﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FireflyUtility.Renderable
{
    public class Camera
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Camera(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
