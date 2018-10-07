using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using System.Threading;

namespace Models
{
    public class World : IObservable<Command>, IUpdatable
    {
        const bool DEBUG = false;

        public List<Model3D> worldObjects = new List<Model3D>();
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();
        public Grid grid = new Grid();
        public Inventory Inventory;
        private bool checkCoordinateShip = false;


        public World()
        {
            DrawRoads(2); // Max 6 roads
            CreateDoors(-35, 4, 12.5);
            CreateDoors(-35, 4, -12.5);
            Inventory = new Inventory(this);
            Thread inventoryPromptThread = new Thread(() => InventoryPrompt(Inventory));
            inventoryPromptThread.Start();

            for (int i = 0; i < 3; i++)
            {
                CreateRobot(grid.GetNodes[i].x, 0.05, grid.GetNodes[i].z);
            }
            CreateSpaceShip(-45, 25, 0);
            CreateModel3D("earth", 500, 10, 500);
        }

        /// <summary>
        /// Prompts the user
        /// </summary>
        /// <param name="inv">The Inventory instace</param>
        private void InventoryPrompt(Inventory inv)
        {
            while (true)
            {
                inv.PromptUser();
            }
        }

        /// <summary>
        /// In this method the object door is created.
        /// </summary>
        /// <param name="x">x-position of the door</param>
        /// <param name="y">y-position of the door</param>
        /// <param name="z">z-position of the door</param>
        /// <returns>The object door</returns>
        private Doors CreateDoors(double x, double y, double z)
        {
            Doors door = new Doors(this, "door", x, y, z, 0, 0, 0);
            worldObjects.Add(door);
            return door;
        }

        /// <summary>
        /// Create a new robot
        /// </summary>
        /// <param name="x">x-position</param>
        /// <param name="y">y-position</param>
        /// <param name="z">z-position</param>
        /// <returns>Returns the robot that was created</returns>
        private Robots CreateRobot(double x, double y, double z)
        {
            Robots robot = new Robots(this, "robot", x, y, z, 0, 0, 0);
            worldObjects.Add(robot);
            return robot;
        }

        /// <summary>
        /// Create a new spaceship
        /// </summary>
        /// <param name="x">x-position</param>
        /// <param name="y">y-position</param>
        /// <param name="z">z-position</param>
        /// <returns>Returns the spaceship that was created</returns>
        private Spaceships CreateSpaceShip(double x, double y, double z)
        {
            Spaceships ship = new Spaceships(this, "spaceship", x, y, z, 0, 0, 0);
            ship.reset();
            worldObjects.Add(ship);
            return ship;
        }

        /// <summary>
        /// Create a new Model3D
        /// </summary>
        /// <param name="x">x-position</param>
        /// <param name="y">y-position</param>
        /// <param name="z">z-position</param>
        /// <returns>Returns the model that was created</returns>
        private Model3D CreateModel3D(string type, double x, double y, double z)
        {
            Model3D model = new Model3D(this, type, x, y, z, 0, 0, 0);
            worldObjects.Add(model);
            return model;
        }

        /// <summary>
        /// Create a new Rack
        /// </summary>
        /// <param name="x">x-position</param>
        /// <param name="y">y-position</param>
        /// <param name="z">z-position</param>
        /// <returns>Returns the rack that was created</returns>
        private Racks CreateRack(Node node)
        {
            Racks rack = new Racks(this, "rack", node.x, 2, node.z, -0.05, -1.42, 0);
            rack.currentNode = node;
            worldObjects.Add(rack);
            return rack;
        }

        /// <summary>
        /// Creates a new light
        /// </summary>
        /// <param name="x">x-position</param>
        /// <param name="y">y-position</param>
        /// <param name="z">z-position</param>
        /// <param name="r_x">x-rotation</param>
        /// <param name="r_y">y-rotation</param>
        /// <param name="r_z">z-rotation</param>
        private void drawLight(double x, double y, double z, double r_x, double r_y, double r_z)
        {
            Model3D light = new Model3D(this, "light", x, y, z, r_x, r_y, r_z);
            worldObjects.Add(light);
        }


        /// <summary>
        /// Creates a new road
        /// </summary>
        /// <param name="x">x-position</param>
        /// <param name="z">z-position</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void drawRoad(double x, double z, double width, double height)
        {
            Model3D road = new Model3D(this, "road", x, 0, z, 0, 0, 0);
            road.Transform(width, height, 0);
            worldObjects.Add(road);
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
                            robot.Update(tick);
                        }
                        else if (u is Spaceships)
                        {
                            Spaceships spaceship = (Spaceships)u;
                            spaceship.needsUpdate = true;

                            // Check if the spaceship is above the building and shipments is not empty
                            if (Inventory.shipments.Count() > 0 && (spaceship.z <= 8 && spaceship.z >= -8))
                            {
                                ReceiveShipment(spaceship);
                            }

                            // Check if the warehouse needs to be restocked after the spaceship passed the building
                            if (spaceship.z < -8)
                                Inventory.CheckStock();

                            // Check if the warehouse has orders
                            if (Inventory.orders.Count() > 0)
                            {
                                checkCoordinateShip = ReceiveCargo(spaceship);
                            }

                            // Move the spaceship if the spaceship has any type of orders or has already begun moving
                            if ((Inventory.orders.Count() > 0 || Inventory.shipments.Count() > 0) || (spaceship.z > -140 ^ spaceship.z == 125))
                                spaceship.moveSpaceship();

                            // If the spaceship has almost reached his destination, you receive your delivery
                            if (spaceship.z == -139 && spaceship.cargo.Count() > 0)
                            {
                                spaceship.cargo.ForEach(x =>
                                    Console.WriteLine("{0} of {1} was delivered to you", x.stock, x.name));

                                spaceship.cargo.Clear();
                            }
                        }
                        else if (u is Racks)
                        {
                            Racks rack = (Racks)u;
                            rack.moveRack();

                            if (rack.attr == "deleted")
                            {
                                worldObjects.Remove(rack);
                                Inventory.RemoveRack(rack);
                            }
                        }

                        else if (u is Doors)
                        {
                            Doors door = (Doors)u;
                            door.Update(tick);
                        }

                        else if (u is Model3D)
                        {
                            Model3D model = (Model3D)u;
                            //If model type is light then call the methode move of light.
                            if (model.type == "light")
                                model.Move(model.x, model.y, model.z);

                            //Calls the methode moveEarth and gives model along with it.
                            moveEarth(model);
                        }

                        SendCommandToObservers(new UpdateModel3DCommand(u));
                    }
                }
            }
            return true;
        }


       
        private double radius = 0;
        /// <summary>
        /// Moves the earth
        /// </summary>
        /// <param name="model">Receives the object model</param>
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


        private int count = 0;
        private Racks rack = null;
        /// <summary>
        /// Checks if the spaceship is in the right coordinates by calling the methode spaceship.checkCooridinates which returns a true or false
        /// if true than it counts count up to 10 (This because otherwise the z coordinates whill be different) if false nothing will be done. If 
        /// count is 10 or bigger than it will create racks if the node is not occupied.
        /// </summary>
        /// <param name="spaceship">Receives the object spaceship</param>
        /// <returns>boolean</returns>
        private bool ReceiveCargo(Spaceships spaceship)
        {
            if (spaceship.checkCoordinates() && count < 11)
            {
                count++;
            }
            
            if (10 <= count)
            {
                while (Inventory.orders.Count != 0)
                {
                    for (int i = 3; i < 8; i++)
                    {
                        if (!grid.GetNodes[i].occupied)
                        {
                            // Create the rack
                            rack = CreateRack(grid.GetNodes[i]);
                            Inventory.AddRack(rack);

                            // Set node to occupied
                            grid.GetNodes[i].occupied = true;

                            // Add products to the rack
                            rack.AddProduct(Inventory.orders.First());
                            Inventory.orders.Remove(Inventory.orders.First());
                            Inventory.AddTask(rack);
                            break;
                        }
                    }
                }
                count = 0;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Spaceship receives the shipment
        /// </summary>
        /// <param name="spaceship"></param>
        private void ReceiveShipment(Spaceships spaceship)
        {
            Task task = null;

            for (int i = 0; i <= Inventory.shipments.Count() - 1; i++) // Each shipment
            {
                for (int j = 0; j <= Inventory.racks.Count() - 1; j++) // Each rack
                {
                    if (Inventory.racks[j].currentNode.type == "cargoNode" && !Inventory.racks[j].moving && !Inventory.orders.Contains(Inventory.shipments[i]))
                    {
                        for (int k = 0; k <= Inventory.racks[j].contains.Count() - 1; k++) // Each product in each rack
                        {
                            if (Inventory.racks[j].contains[k].id == Inventory.shipments[i].id)
                            {
                                spaceship.AddCargo(Inventory.shipments[i]);
                                task = Inventory.racks[j].RemoveStock(Inventory.racks[j].contains[k], Inventory.shipments[i].stock);
                                Inventory.shipments.Remove(Inventory.shipments[i]);

                                if (task != null)
                                    Inventory.Tasks.Add(task);

                                if (Inventory.shipments.Count() == 0)
                                    return;
                            }
                        }

                        // Delete the rack
                        if (Inventory.racks[j].contains.Count() == 0)
                        {
                            Inventory.racks[j].currentNode.occupied = false;
                            Inventory.racks[j].attr = "deleted";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method makes roads based on a given amount. It will also draw light, if there is one more road then there will also be 
        /// one more light etc.
        /// </summary>
        /// <param name="amountRoads">How many roads the methode as to draw</param>
        private void DrawRoads(double amountRoads)
        {
            double x = 5, z = 20, width = 82, height = 2; //Starting point and standard values
            string type = "";

            // Draw the roads over the x-axis
            drawRoad(x, z, width, height);
            drawRoad(x, -z, width, height);

            double percent = 1 / amountRoads;
            double length = width;
            double startPosition = -35;
            double segment = length * percent;

            // Draw the roads in the middle
            for (int i = 0; i <= amountRoads; i++)
            {
                drawRoad(startPosition + segment * i, 0, height, width / 2);
                drawLight(startPosition + segment * i, 10, 12, 0, 1.57, 0);
                for (int j = 0; j <= 10; j++)
                {
                    type = "";
                    if (i == 0 && (j > 2 && j < 8))
                        type = "cargoNode";
                    else if (i > 0 && (j > 0 && j < 10))
                        type = "storageNode";

                    Node rackNode = grid.AddNode(startPosition + segment * i, 20 - 40 / 10 * j, type);
                }
            }


            addConnections();
            if (DEBUG)
                drawNodes();
            Console.WriteLine("Loading road...");
        }

        /// <summary>
        /// Add the synapses
        /// </summary>
        public void addConnections()
        {
            for (int i = grid.GetNodes.Count - 1; i >= 0; i--)
            {
                // Skip the nodes at the ends
                if (i % 11 != 0)
                    grid.AddConnection(grid.GetNodes[i], grid.GetNodes[i - 1]); // Create synapses from back to start

                // Connect the ends to eachother
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

            // Create the synapses from start to back
            for (int i = 1; i < grid.GetNodes.Count; i++)
            {
                if (i % 11 != 0 || i == 0)
                    grid.AddConnection(grid.GetNodes[i - 1], grid.GetNodes[i]);
            }
        }

        /// <summary>
        /// Draw nodes and synapses on screen
        /// </summary>
        private void drawNodes()
        {
            foreach (Node node in grid.GetNodes)
            {
                Model3D model = CreateModel3D("node", node.x, 0, node.z);
                model.attr = node.type;
            }

            foreach (ConnectedNodes connectedNodes in grid.GetConnectedNodes)
            {
                Model3D synapse = CreateModel3D("synapse", connectedNodes.Source.x, 0, connectedNodes.Source.z);
                synapse.Transform(connectedNodes.Destination.x, 0, connectedNodes.Destination.z);
                worldObjects.Add(synapse);
            }
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


