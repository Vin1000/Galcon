using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIServer
{
    public class AI
    {
        public List<Planet> Planets { get; set; }

        private bool _firstDataReceived = false;

        public TCPServer Game { get; private set; }

        // Set your team Name here!!!
        private const string name = "DarthMinions";

        #region Info
        public List<Planet> MyPlanets
        {
            get
            {
                return this.Planets.Where(p => p.Owner == name).ToList();
            }
        }

        public List<Planet> NeutralPlanets
        {
            get
            {
                return this.Planets.Where(p => p.Owner == string.Empty).ToList();
            }
        }

        public List<Planet> EnemyPlanets
        {
            get
            {
                return this.Planets.Where(p => p.Owner != string.Empty && p.Owner != name).ToList();
            }
        }

        /*public List<Ship> MyShips
        {
            get
            {
                return this.Ships.Where(s => s.Owner == this.MyName).ToList();
            }
        }

        public List<Ship> EnemyShips
        {
            get
            {
                return this.Ships.Where(s => s.Owner != this.MyName).ToList();
            }
        }*/

        #endregion

        public AI(TCPServer server)
        {
            Game = server;
        }

        Planet enemyMaster;
        int attackCount = 0;
        public void update(UpdateContainer container)
        {

            Console.Out.WriteLine("Updating");
            var analyser = new Analyser(container, name);

            if (!_firstDataReceived)
            {
                InitPlanets(container.Planets);
                this._firstDataReceived = true;
                enemyMaster = EnemyPlanets.FirstOrDefault();
            }

            foreach (var planet in Planets)
            {
                foreach (var closest in enemyMaster.GetClosestPlanets(3, false))
                {
                    Game.AttackPlanet(planet, closest, (int)Math.Floor((double)planet.ShipCount*0.8/3));
                }
            }

            /*
            foreach (var planet in analyser.MyPlanets)
            {
                foreach (var idle in analyser.NeutralPlanets)
                {
                    Game.AttackPlanet(planet, idle, (int)Math.Floor((double)planet.ShipCount/analyser.NeutralPlanets.Count));
                }
            }
            if (analyser.NeutralPlanets.Count == 0)
            {
                foreach (var planet in analyser.MyPlanets)
                {
                    foreach (var enemy in analyser.EnemyPlanets)
                    {
                        Game.AttackPlanet(planet, enemy, planet.ShipCount-1);
                    }
                }
            }*/
            attackCount++;
        }

        void InitPlanets(List<Planet> planets)
        {
            this.Planets = new List<Planet>();
            foreach (var planet in planets)
            {

                foreach (var neighbour in planets)
                {
                    planet.PlanetDistances.Add(neighbour, planet.GetDistance(neighbour));
                }
                this.Planets.Add(planet);
            }
        }

        public void set_name()
        {
            Console.Out.WriteLine("Setting name");
            Game.SetName(name);
        }
    }
}
