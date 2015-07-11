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
        private const string name = "Bot";

        public AI(TCPServer server)
        {
            Game = server;
        }

        public void update(UpdateContainer container)
        {

            Console.Out.WriteLine("Updating");
            var analyser = new Analyser(container, name);

            if (!_firstDataReceived)
            {
                InitPlanets(container.Planets);
                this._firstDataReceived = true;
            }

            foreach (var planet in analyser.MyPlanets)
            {
                foreach (var closest in planet.GetClosestPlanets(3))
                {
                    Game.AttackPlanet(planet, closest, 1);

                }
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
