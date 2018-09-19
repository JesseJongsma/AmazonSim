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
            CreateRobot(4.6, 0, 13);
            CreateSpaceShip(-45, 25, 0);
            CreateModel3D("earth", 500, 10, 500);
            // function createRoad(length, depth, x, y, z)
            // createRoad(82, 2, 5, 20);
            // createRoad(82, 2, 5, -20);
            drawRoads(5);
        }

        private Robots CreateRobot(double x, double y, double z)
        {
            Robots robot = new Robots("robot", x, y, z, 0, 0, 0);
            worldObjects.Add(robot);
            return robot;
        }

        private Spaceships CreateSpaceShip(double x, double y, double z)
        {
            Spaceships ship = new Spaceships("spaceship", x, y, z, 0, 0, 0);
            worldObjects.Add(ship);
            return ship;
        }

        private Model3D CreateModel3D(string type, double x, double y, double z)
        {
            Model3D model = new Model3D(type, x, y, z, 0, 0, 0);
            worldObjects.Add(model);
            return model;
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
                        if (u is Robots)
                        {
                            Robots robot = (Robots)u;
                            robot.moveRobot(u);
                        }
                        else if (u is Spaceships)
                        {
                            Spaceships spaceship = (Spaceships)u;
                            spaceship.moveSpaceship(u);
                        }
                        else if (u is Model3D)
                        {
                            Model3D earth = (Model3D)u;
                            moveEarth(earth);
                        }
                        SendCommandToObservers(new UpdateModel3DCommand(u));
                    }
                }
            }

            return true;
        }

        private double radius = 0;
        private void moveEarth(Model3D model)
        {
            if (model.type == "earth")
            {
                model.Rotate(-0.1, 0, -0.1);
                model.Rotate(model.rotationX, radius, model.rotationZ);
                model.Move(model.x, model.y, model.z);
                radius = radius + 0.01;
                radius = (radius >= 360) ? 0 : radius;
            }
        }

        private void drawRoads(double amountRoads)
        {
            double x = 5, z = 20, width = 82, height = 2; //Starting point and standard values
            //function loadRoad(x, y, z)
            // length
            //drawRoad(82, 2, 5, 20, 0);
            //drawRoad(82, 2, 5, -20, 0);
            drawRoad(x, z, width, height);
            drawRoad(x, z * -1, width, height);

            double percent = 1 / amountRoads;
            double length = width;
            double startPosition = -35;
            double segment = length * percent;

            Console.WriteLine(percent);
            for (int i = 0; i <= amountRoads; i++)
            {
                drawRoad(startPosition + segment * i, 0, height, width / 2);
            }
            Console.WriteLine("Loading road...");
        }

        private void drawRoad(double x, double z, double width, double height)
        {
            Model3D road = new Model3D("road", x, 0, z, 0, 0, 0);
            road.Transform(width, height, 0);
            worldObjects.Add(road);
            SendCommandToObservers(new UpdateModel3DCommand(road));
        }
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
