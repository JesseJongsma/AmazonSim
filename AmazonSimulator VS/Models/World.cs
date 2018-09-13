using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using System.Threading; 

namespace Models {
    public class World : IObservable<Command>, IUpdatable
    {
        private List<Model3D> worldObjects = new List<Model3D>();
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();
        private int c = 0;
        private double tr_x = -35, tr_y = 0.05, tr_z = 125; 
        
        public World() {
            Model3D r = CreateRobot(0,0,0);
            r.Move(4.6, 0, 13);

            Model3D t = CreateTruck(0,0,0);
            t.Move(tr_x, tr_y, tr_z);
        }

        private Model3D CreateRobot(double x, double y, double z) {
            Model3D r = new Model3D("robot",x,y,z,0,0,0);
            worldObjects.Add(r);
            return r;
        }

        private Model3D CreateTruck(double x, double y, double z)
        {
            Model3D t = new Model3D("truck", x, y, z, 0, 0, 0);
            worldObjects.Add(t);
            return t; 
        }

        //private Model3D CreateRack(double x, double y, double z)
        //{
        //    Model3D Rack = new Model3D("rack", x, y, z, 0, 0, 0);
        //    worldObjects.Add(Rack);
        //    return Rack;
        //}


        public IDisposable Subscribe(IObserver<Command> observer)
        {
            if (!observers.Contains(observer)) {
                observers.Add(observer);

                SendCreationCommandsToObserver(observer);
            }
            return new Unsubscriber<Command>(observers, observer);
        }

        private void SendCommandToObservers(Command c) {
            for(int i = 0; i < this.observers.Count; i++) {
                this.observers[i].OnNext(c);
            }
        }

        private void SendCreationCommandsToObserver(IObserver<Command> obs) {
            foreach(Model3D m3d in worldObjects) {
                obs.OnNext(new UpdateModel3DCommand(m3d));
            }
        }

        public bool Update(int tick)
        {
            for(int i = 0; i < worldObjects.Count; i++) {

                Model3D u = worldObjects[i];
                
                if(u is IUpdatable) {
                    bool needsCommand = ((IUpdatable)u).Update(tick);

                    if(needsCommand) {
                        moveRobot(u);
                        moveTruck(u);
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

        private void moveTruck(Model3D model)
        {
            if (model.type == "truck")
            {
                tr_z = tr_z - 0.5;
                if (tr_z == 12.5)
                    Thread.Sleep(5000);

                if (tr_z == -140)
                    goto Stop;

                model.Move(tr_x, tr_y, tr_z);
            }
        Stop:;
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
