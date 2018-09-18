using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using System.Threading;

namespace Models
{
    public class World : IObservable<Command>, IUpdatable
    {
        private List<Model3D> worldObjects = new List<Model3D>();
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();
        private int c = 0;

        public World()
        {
            Robots robot = CreateRobot(4.6, 0, 13);
            Spaceships ship = CreateSpaceShip(-20, 25, 0);
            Model3D earth = CreateModel3D("earth", 500, 10,500);
        }

        private Robots CreateRobot(double x, double y, double z)
        {
            Robots robot = new Robots("robot", x, y, z, 0, 0, 0);
            worldObjects.Add(robot);
            return robot;
        }

        private Spaceships CreateSpaceShip(double x, double y, double z)
        {
            Spaceships ship = new Spaceships("starship", x, y, z, 0, 0, 0);
            worldObjects.Add(ship);
            return ship;
        }

        private Model3D CreateModel3D(string type, double x, double y, double z)
        {
            Model3D model = new Model3D(type, x, y, z, 0, 0, 0);
            worldObjects.Add(model);
            return model;
        }

        private void DrawLine()
        {

        }

        private void DrawRoad()
        {

        }

        //private Model3D CreateRack(double x, double y, double z)
        //{
        //    Model3D Rack = new Model3D("rack", x, y, z, 0, 0, 0);
        //    worldObjects.Add(Rack);
        //    return Rack;
        //}


        public IDisposable Subscribe(IObserver<Command> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);

                SendCreationCommandsToObserver(observer);
            }
            return new Unsubscriber<Command>(observers, observer);
        }

        private void SendCommandToObservers(Command c)
        {
            for (int i = 0; i < this.observers.Count; i++)
            {
                this.observers[i].OnNext(c);
            }
        }

        private void SendCreationCommandsToObserver(IObserver<Command> obs)
        {
            foreach (Model3D m3d in worldObjects)
            {
                obs.OnNext(new UpdateModel3DCommand(m3d));
            }
        }

        public bool Update(int tick)
        {
            for (int i = 0; i < worldObjects.Count; i++)
            {

                Model3D u = worldObjects[i];

                if (u is IUpdatable)
                {
                    bool needsCommand = ((IUpdatable)u).Update(tick);

                    if (needsCommand)
                    {
                        moveRobot(u);
                        moveSpaceship(u);
                        SendCommandToObservers(new UpdateModel3DCommand(u));
                    }
                }
            }

            return true;
        }

        private void moveRobot(Model3D model)
        {
            if (model.type == "robot")
            {
                c++;
                if (c > 100)
                    c = 0;

                model.Move(c, 0, c);
            }
        }
        private double ss_z = 125;
        private int count = 0;
        private double radius = 0;
        private void moveSpaceship(Model3D model)
        {
            if (model.type == "starship" && ss_z != -140)
            {
                if (ss_z == 12.5 && count != 100)
                {
                    count++;
                    model.Move(model.x, 25, model.z); // Set needsUpdate back to true
                }
                else
                {
                    ss_z -= 0.5;
                    model.Move(model.x, 25, ss_z);
                }

                model.Move(model.x, model.y + Math.Cos(radius) * 1.5, model.z);
                model.Rotate(model.rotationX, radius / 2, model.rotationZ);
                radius += 0.25;
                radius = (radius >= 360) ? 0 : radius; // reset radius
            }
            else if (ss_z == -140) // reset the starship to the initial position
            {
                ss_z = 125;
                model.Move(-20, 25, ss_z);
                count = 0;
            }
        }

        private void moveEarth(Model3D model)
        {
            if (model.type == "earth")
            {
                model.Rotate(model.rotationX, radius / 2, model.rotationZ);
                radius = (radius >= 360) ? 0 : radius; // reset radius
            }
        }

        private void DrawCone()
        {
            
        }
    }



    internal class Unsubscriber<Command> : IDisposable
    {
        private List<IObserver<Command>> _observers;
        private IObserver<Command> _observer;

        internal Unsubscriber(List<IObserver<Command>> observers, IObserver<Command> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}
