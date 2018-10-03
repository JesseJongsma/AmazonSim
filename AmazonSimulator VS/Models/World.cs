using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using System.Threading;

namespace Models
{
    public class World : IObservable<Command>, IUpdatable
    {
        public List<Model3D> worldObjects = new List<Model3D>();
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();
        public Grid grid = new Grid();
        public List<Racks> racks = new List<Racks>();
        public Inventory Inventory;
        private bool tasksLoaded = false;
        private bool checkCoordinateShip = false;

        public World()
        {
            DrawRoads(2); // Max 6 roads
            
            Inventory = new Inventory(this);
            Thread inventoryThread = new Thread(() => InventoryPrompt(Inventory));
            inventoryThread.Start();

            for (int i = 0; i < 3; i++)
            {
                CreateRobot(grid.GetNodes[i].x, 0.05, grid.GetNodes[i].z);
            }
            CreateSpaceShip(-45, 25, 0);
            CreateModel3D("earth", 500, 10, 500);
        }

        public void InventoryPrompt(Inventory inv)
        {
            while (true)
            {
                inv.PromptUser(this);
            }
        }

        private Robots CreateRobot(double x, double y, double z)
        {
            Robots robot = new Robots(this, "robot", x, y, z, 0, 0, 0);
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

        private Racks CreateRack(Node node)
        {
            Racks rack = new Racks("rack", node.x, 2, node.z, -0.05, -1.42, 0);
            rack.currentNode = node;
            worldObjects.Add(rack);
            return rack;
        }

        private void drawLight(double x, double y, double z, double r_x, double r_y, double r_z)
        {
            Model3D light = new Model3D("light", x, y, z, r_x, r_y, r_z);
            worldObjects.Add(light);
        }

        private void drawRoad(double x, double z, double width, double height)
        {
            Model3D road = new Model3D("road", x, 0, z, 0, 0, 0);
            road.Transform(width, height, 0);
            worldObjects.Add(road);
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

        private int countRacks = 0; 
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
                            foreach(Model3D model in worldObjects)
                            {
                                if(model.type == "rack")
                                    countRacks++; 
                            }

                            if (countRacks != 0)
                            {
                                Robots robot = (Robots)u;
                                //if (robot.robotMove == null && Inventory.Tasks.Count != 0)
                                //    robot.giveTask(addTask(robot));
                                robot.Update(tick);
                            }
                        }
                        else if (u is Spaceships)
                        {
                            Spaceships spaceship = (Spaceships)u;
                            spaceship.moveSpaceship();
                            if (Inventory.orders.Count > 0)
                            {
                                checkCoordinateShip = ReceiveCargo(spaceship);
                            }
                        }
                        else if (u is Racks)
                        {
                            Racks rack = (Racks)u;
                            rack.moveRack();
                            if (checkCoordinateShip == true && tasksLoaded == false)
                            {
                                tasksLoaded = true;
                            }
                        }

                        else if(u is Model3D)
                        {
                            Model3D model = (Model3D)u;
                            if(model.type == "light")
                                model.Move(model.x, model.y, model.z);

                            moveEarth(model);
                        }


                        SendCommandToObservers(new UpdateModel3DCommand(u));
                    }
                }
            }

            return true;
        }

        private Task addTask(Robots robot)
        {
            Task task = Inventory.Tasks.First();
            Inventory.Tasks.RemoveAt(0);
            return task;
        }

        private double radius = 0;
        private void moveEarth(Model3D model)
        {
            if (model.type == "earth")
            {
                model.Rotate(-0.15, 0, -0.15);
                model.Rotate(model.rotationX, radius, model.rotationZ);
                model.Move(model.x, model.y, model.z);
                radius = radius + 0.0025;
                radius = (radius >= 360) ? 0 : radius;
            }
        }


        private int loaded = 0;
        private int i = 3;
        private bool ReceiveCargo(Spaceships spaceship)
        {
            Racks rack = null;
            if (spaceship.checkCoordinates())
            {
                loaded++;
            }

            if (10 <= loaded)
            {
                for (int j = 0; j <= Inventory.orders.Count - 1; j++)
                {
                    if (j % 5 == 0)
                    {
                        rack = CreateRack(grid.GetNodes[i]);
                    }
                    rack.AddProduct(Inventory.orders[j]);
                    Inventory.AddTask(rack);
                    Inventory.orders.Remove(Inventory.orders[j]);
                }
                
                i++;
                loaded = 0;
                return true;
            }
            else
                return false;
        }

        private void DrawRoads(double amountRoads)
        {
            double x = 5, z = 20, width = 82, height = 2; //Starting point and standard values
            string type;

            drawRoad(x, z, width, height);
            drawRoad(x, -z, width, height);

            double percent = 1 / amountRoads;
            double length = width;
            double startPosition = -35;
            double segment = length * percent;

            for (int i = 0; i <= amountRoads; i++)
            {
                drawRoad(startPosition + segment * i, 0, height, width / 2);
                drawLight(startPosition + segment * i, 10, 12, 0, 1.57, 0);
                for (int j = 0; j <= 10; j++)
                {
                    if (i == 0 && (j > 2 && j < 9))
                        type = "cargoNode";
                    else
                        type = "storageNode";

                    Node rackNode = grid.AddNode(startPosition + segment * i, 20 - 40 / 10 * j, type);

                    //RackNodes.Add(rackNode);
                }
            }

            drawNodes();
            addConnections();
            Console.WriteLine("Loading road...");
        }

        public void addConnections()
        {
            for (int i = grid.GetNodes.Count - 1; i >= 0; i--)
            {
                if (i % 11 != 0)
                    grid.AddConnection(grid.GetNodes[i], grid.GetNodes[i - 1]);

                if (grid.GetNodes[i].z == 20 || grid.GetNodes[i].z == -20)
                {
                    if (i >= 11)
                    {
                        grid.AddConnection(grid.GetNodes[i], grid.GetNodes[i - 11]);
                    }

                    if (i < grid.GetNodes.Count - 11)
                    {
                        grid.AddConnection(grid.GetNodes[i], grid.GetNodes[i + 11]);
                    }
                }

            }

            for (int i = 1; i < grid.GetNodes.Count; i++)
            {
                if (i % 11 != 0 || i == 0)
                    grid.AddConnection(grid.GetNodes[i - 1], grid.GetNodes[i]);
            }

            foreach (ConnectedNodes connectedNodes in grid.GetConnectedNodes)
            {
                Model3D synapse = CreateModel3D("synapse", connectedNodes.Source.x, 0, connectedNodes.Source.z);
                synapse.Transform(connectedNodes.Destination.x, 0, connectedNodes.Destination.z);
                worldObjects.Add(synapse);
            }
        }
        
        private void drawNodes()
        {
            foreach (Node node in grid.GetNodes)
            {
                CreateModel3D("node", node.x, 0, node.z);
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


