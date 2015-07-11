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
        private const string name = "Darth Minions";

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
        int attackStrategy = 0;
        public void update(UpdateContainer container)
        {

            Console.Out.WriteLine("Updating");

            if (!_firstDataReceived)
            {
                InitPlanets(container.Planets);
                this._firstDataReceived = true;
            }
            else
            {
                //mettre a jour
                this.UpdatePlanets(container.Planets);
            }
            enemyMaster = EnemyPlanets.OrderBy(p => p.ShipCount).FirstOrDefault();

            int planetsToAttack = 2;
            var percentage = 0.8;
            var closestToEnemyMaster = enemyMaster.GetClosestPlanets(planetsToAttack, false);
            switch (attackStrategy)
            {
                case 0:
                    foreach (var planet in MyPlanets)
                    {
                        foreach (var closest in closestToEnemyMaster)
                        {
                            Game.AttackPlanet(planet, closest, (int)Math.Floor((double)planet.ShipCount * percentage / planetsToAttack));
                        }
                    }
                    if (!closestToEnemyMaster.Any(p => p.Owner != name))
                    {
                        attackStrategy = 1;
                    }
                    break;
                case 1:
                    if (closestToEnemyMaster.Any(p => p.Owner != name))
                    {
                        attackStrategy = 0;
                    }
                    foreach (var planet in MyPlanets)
                    {
                        Game.AttackPlanet(planet, enemyMaster, (int)Math.Floor((double)planet.ShipCount * percentage / planetsToAttack));
                    }
                    break;
                default:
                    break;
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

        void UpdatePlanets(List<Planet> newPlanets)
        {
            foreach (var newPlanet in newPlanets)
            {
                var currentPlanet = this.Planets.First(p => p.Id == newPlanet.Id);
                currentPlanet.Owner = newPlanet.Owner;
                currentPlanet.ShipCount = newPlanet.ShipCount;
            }
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
