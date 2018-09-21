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
        public Nodes nodes = new Nodes();
        private int c = 0;
        private int cargo = 3; //Number of receiving racks

        public World()
        {

            drawRoads(6); // Max 6 roads
            CreateRobot(0, 0.05, 0);
            CreateSpaceShip(-45, 25, 0);
            CreateModel3D("earth", 500, 10, 500);
        }

        private Robots CreateRobot(double x, double y, double z)
        {
            Robots robot = new Robots(nodes, "robot", x, y, z, 0, 0, 0);
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

        private Racks CreateRack(double x, double y, double z)
        {
            Racks rack = new Racks("rack", x, y, z, -0.05, -1.42, 0);
            worldObjects.Add(rack);
            return rack;
        }

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
                            robot.moveRobot();
                        }
                        else if (u is Spaceships)
                        {
                            Spaceships spaceship = (Spaceships)u;
                            spaceship.moveSpaceship();
                            receiveCargo(spaceship);
                        }
                        else if (u is Racks)
                        {
                            Racks rack = (Racks)u;
                            rack.moveRack();
                        }

                        else
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
        
        private int loaded = 0;
        private double x = 0; 
        private void receiveCargo(Spaceships spaceship)
        {
            if (spaceship.checkCoordinates())
                loaded++;

            if (10 < loaded && loaded <= (cargo + 10)) 
            {
                Racks rack = CreateRack(spaceship.x + x, 2, spaceship.z);
                Console.WriteLine("LOADING RACK");
                x+=2.5; 
            }
        }

        private void drawRoads(double amountRoads)
        {
            List<Nodes> CornerNodesSRC = new List<Nodes>();
            List<Nodes> CornerNodesDES = new List<Nodes>();
            List<Nodes> RackNodes = new List<Nodes>();

            double x = 5, z = 20, width = 82, height = 2; //Starting point and standard values

            drawRoad(x, z, width, height);
            drawRoad(x, -z, width, height);

            //nodes.AddNode(width / 2 + 6, z);
            //nodes.AddNode(width / 2 + 6, -z);

            double percent = 1 / amountRoads;
            double length = width;
            double startPosition = -35;
            double segment = length * percent;

            for (int i = 0; i <= amountRoads; i++)
            {
                drawRoad(startPosition + segment * i, 0, height, width / 2);
                Nodes destination = nodes.AddNode(startPosition + segment * i, z);
                Nodes source = nodes.AddNode(startPosition + segment * i, -z);

                CornerNodesDES.Add(destination);
                CornerNodesSRC.Add(source);

                for (int j = 1; j < 10; j++)
                {
                    Nodes rackNode = nodes.AddNode(startPosition + segment * i, 20 - 40 / 10 * j);
                    RackNodes.Add(rackNode);
                }
            }
            
            drawNodes();
            addConnections(CornerNodesSRC, CornerNodesDES, RackNodes);
            Console.WriteLine("Loading road...");
        }

        private void addConnections(List<Nodes> cornerNodesSRC, List<Nodes> cornerNodesDES, List<Nodes> rackNodes)
        {
            for (int i = 0; i < cornerNodesSRC.Count; i++)
            {
                for (int j = 0; j < cornerNodesDES.Count; j++)
                {
                    if (i != 0 && j != 0)
                    {
                        // Vertical lines
                        nodes.AddConnection(cornerNodesSRC[i], cornerNodesSRC[i - 1]); // Right side
                        nodes.AddConnection(cornerNodesDES[j], cornerNodesDES[j - 1]); // Left side
                    }
                    // Connect vertical lines with horizontal lines
                    nodes.AddConnection(cornerNodesSRC[i], rackNodes[i * 9 + 8]); // Right side
                    nodes.AddConnection(cornerNodesDES[j], rackNodes[j * 9]); // left side
                }
            }

            for (int r = 0; r < rackNodes.Count; r++)
            {
                if (r != 0 && r % 9 != 0)
                    nodes.AddConnection(rackNodes[r - 1], rackNodes[r]);
            }

            foreach (ConnectedNodes connectedNodes in nodes.GetConnectedNodes)
            {
                foreach (Nodes destination in connectedNodes.Destinations)
                {
                    Model3D synapse = CreateModel3D("synapse", connectedNodes.Source.GetX, 0, connectedNodes.Source.GetZ);
                    synapse.Transform(destination.GetX, 0, destination.GetZ);
                    worldObjects.Add(synapse);
                }
            }
        }

        private void drawRoad(double x, double z, double width, double height)
        {
            Model3D road = new Model3D("road", x, 0, z, 0, 0, 0);
            road.Transform(width, height, 0);
            worldObjects.Add(road);
        }

        private void drawNodes()
        {
            foreach (Nodes node in nodes.GetNodes)
            {
                CreateModel3D("node", node.GetX, 0, node.GetZ);
                
            }
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
