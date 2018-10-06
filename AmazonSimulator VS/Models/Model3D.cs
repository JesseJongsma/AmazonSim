using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Models {
    public class Model3D : IUpdatable {
        private double _x = 0;
        private double _y = 0;
        private double _z = 0;
        private double _rX = 0;
        private double _rY = 0;
        private double _rZ = 0;
        private double _width = 0;
        private double _height = 0;
        private double _depth = 0;

        protected World world;

        public string type { get; }
        public string attr;
        public Guid guid { get; }
        public double x { get { return _x; } set { _x = value; } }
        public double y { get { return _y; } set { _y = value; } }
        public double z { get { return _z; } set { _z = value; } }
        public double rotationX { get { return _rX; } }
        public double rotationY { get { return _rY; } }
        public double rotationZ { get { return _rZ; } }
        public double width { get { return _width; } }
        public double height { get { return _height; } }
        public double depth { get { return _depth; } }

        public bool needsUpdate = true;

        /// <summary>
        /// The constructor of the model. Sets the private variables.
        /// </summary>
        /// <param name="type">Says which type of model it is</param>
        /// <param name="x">Sets x coordinate of the model</param>
        /// <param name="y">Sets y coordinate of the model</param>
        /// <param name="z">Sets z coordinate of the model</param>
        /// <param name="rotationX">Sets the rotation x in radians of the model</param>
        /// <param name="rotationY">Sets the rotation y in radians of the model</param>
        /// <param name="rotationZ">Sets the rotation z in radians of the model</param>
        public Model3D(World world, string type, double x, double y, double z, double rotationX, double rotationY, double rotationZ) {
            this.world = world;
            this.type = type;
            this.guid = Guid.NewGuid();

            this._x = x;
            this._y = y;
            this._z = z;

            this._rX = rotationX;
            this._rY = rotationY;
            this._rZ = rotationZ;
        }

        /// <summary>
        /// If you want to move a model than call this methode. It also sets needsUpdate to true, so the movement in the back-end will 
        /// also be in the front-end. 
        /// </summary>
        /// <param name="x">Sets x coordinate of the model</param>
        /// <param name="y">Sets y coordinate of the model</param>
        /// <param name="z">Sets z coordinate of the model</param>
        public virtual void Move(double x, double y, double z) {
            this._x = x;
            this._y = y;
            this._z = z;

            needsUpdate = true;
        }

        /// <summary>
        ///  If you want to rotate a model than call this methode. It also sets needsUpdate to true, so the movement in the back-end will 
        /// also be in the front-end. 
        /// </summary>
        /// <param name="rotationX">Sets the rotation x in radians of the model</param>
        /// <param name="rotationY">Sets the rotation y in radians of the model</param>
        /// <param name="rotationZ">Sets the rotation z in radians of the model</param>
        public virtual void Rotate(double rotationX, double rotationY, double rotationZ) {
            this._rX = rotationX;
            this._rY = rotationY;
            this._rZ = rotationZ;

            needsUpdate = true;
        }

        /// <summary>
        /// If you want to make a model bigger, higher or deeper than call this methode. 
        /// </summary>
        /// <param name="width">Sets the width of the model</param>
        /// <param name="height">Sets the height of the model</param>
        /// <param name="depth">Sets the depth of the model</param>
        public virtual void Transform(double width, double height, double depth)
        {
            _width = width;
            _height = height;
            _depth = depth;
        }

        /// <summary>
        /// Is checking if a model is needed to be updated.
        /// </summary>
        /// <param name="tick">How many times it has to check the methode update </param>
        /// <returns> true or false</returns>
        public virtual bool Update(int tick)
        {
            if(needsUpdate) {
                needsUpdate = false;
                return true;
            }
            return false;
        }
    }
}