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

        public Planet DeathStar { get; set; }

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
        int previousStrategy = 0;
        int planetsToAttack = 2;
        double percentage = 0.65;
        List<Planet> closestToEnemyMaster = new List<Planet>();
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
            closestToEnemyMaster = enemyMaster.GetClosestPlanets(planetsToAttack, false);

            Game.DeathstarDestroyPlanet(DeathStar, enemyMaster);

            if (DeathStar.ReadyToAttackNextTurn)
            {
                attackStrategy = 3;
                Console.Out.WriteLine("ready = 3");
            }

            switch (attackStrategy)
            {
                case 0:
                    Strategy0();
                    break;
                case 1:
                    Strategy1();
                    break;
                case 2:
                    Strategy2();
                    break;
                case 3:
                    Strategy3();
                    break;
                default:
                    Console.Out.WriteLine("Error attackStrategy: " + attackStrategy);
                    break;
            }
            attackCount++;
        }

        void Strategy0() //attack noob planets and enemy master neighbors
        {
            foreach (var planet in MyPlanets)
            {
                foreach (Planet p in Planets.Where(p => p.ShipCount <= 5).ToList()) //sauf neighbor!!!
                {
                    try
                    {
                        Game.AttackPlanet(MyPlanets.First(), p, 6);
                    }
                    catch (Exception)
                    {
                        Console.Out.WriteLine("Planet attack sucked: " + p.Id);
                    }
                    
                }
                foreach (var closest in closestToEnemyMaster)
                {
                    try
                    {
                        Game.AttackPlanet(planet, closest, (int)Math.Floor((double)planet.ShipCount * percentage / planetsToAttack));
                    }
                    catch (Exception)
                    {
                        Console.Out.WriteLine("Closest Planet attack sucked: " + closest.Id);
                    }
                }
            }
            attackStrategy = 1;
        }

        void Strategy1() //attack enemy master neighbors
        {
            foreach (var planet in MyPlanets)
            {
                foreach (var closest in closestToEnemyMaster)
                {
                    try
                    {
                        Game.AttackPlanet(planet, closest, (int)Math.Floor((double)planet.ShipCount * percentage / planetsToAttack));
                    }
                    catch (Exception)
                    {
                        Console.Out.WriteLine("Closest Planet attack sucked: " + closest.Id);
                    }
                }
            }
            if (!closestToEnemyMaster.Any(p => p.Owner != name))
            {
                attackStrategy = 2;
            }
        }

        void Strategy2() //attack enemy master
        {
            foreach (var planet in MyPlanets)
            {
                try
                {
                    Game.AttackPlanet(planet, enemyMaster, (int)Math.Floor((double)planet.ShipCount * percentage / planetsToAttack));
                }
                catch (Exception)
                {
                    Console.Out.WriteLine("Planet attack sucked: " + planet.Id);
                }
            }
            if (closestToEnemyMaster.Any(p => p.Owner != name))
            {
                attackStrategy = 1;
            }
        }
        
        void Strategy3() //avoid death star
        {
            Console.Out.WriteLine("!");
            foreach (var planet in MyPlanets)
            {
                var myClosestPlanets = planet.GetMyClosestPlanets(name);
                int no = 0;
                if(myClosestPlanets.Count >= 2)
                {
                    no = 1;
                }
                if(myClosestPlanets.Any())
                {
                    try
                    {
                        Game.AttackPlanet(planet, myClosestPlanets[no], (int)Math.Floor((double)planet.ShipCount * (1 - percentage)));
                    }
                    catch (Exception)
                    {
                        Console.Out.WriteLine("Planet attack sucked: " + planet.Id);
                    }
                }

                try
                {
                    Game.AttackPlanet(planet, enemyMaster, (int)Math.Ceiling((double)planet.ShipCount * percentage) - 1);
                }
                catch (Exception)
                {
                    Console.Out.WriteLine("Planet attack sucked: " + planet.Id);
                }
                
            }
            attackStrategy = 2;
            Console.Out.WriteLine("strat = 2");
        }

        void UpdatePlanets(List<Planet> newPlanets)
        {

            if (newPlanets.Count != Planets.Count)
            {
                this.Planets = this.Planets.Where(p => newPlanets.Any(np => np.Id == p.Id)).ToList();
            }
            
            foreach (var newPlanet in newPlanets)
            {
                var currentPlanet = this.Planets.First(p => p.Id == newPlanet.Id);
                currentPlanet.Update(newPlanet.Owner, newPlanet.ShipCount, newPlanet.DeathStarCharge);
            }
        }

        void InitPlanets(List<Planet> planets)
        {
            this.DeathStar = planets.FirstOrDefault(p => p.IsDeathStar);
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
