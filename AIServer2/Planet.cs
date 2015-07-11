using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace AIServer
{
    public class Planet
    {
        public int Id { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public string Owner { get; set; }
        public int ShipCount { get; set; }
        public float Size { get; set; }
        public float DeathStarCharge { get; set; }
        public bool IsDeathStar { get; set; }

        public Dictionary<Planet, float> PlanetDistances { get; set; }

        public List<Planet> GetClosestPlanets(int number, bool includeSelf)
        {
            number = Math.Min(PlanetDistances.Count - 1, number);
            if (includeSelf)
            {
                return PlanetDistances.Keys.OrderBy(p => this.GetDistance(p)).Take(number).ToList();
            }
            return PlanetDistances.Keys.Where(p => p.Id != this.Id).OrderBy(p => this.GetDistance(p)).Take(number).ToList();
        }

        public Planet(dynamic json)
        {
            Id = Int32.Parse(json.id);
            Size = float.Parse(json.size, CultureInfo.InvariantCulture);
            ShipCount = Int32.Parse(json.ship_count);
            Owner = json.owner;
            PosX = float.Parse(json.position.x, CultureInfo.InvariantCulture);
            PosY = float.Parse(json.position.y, CultureInfo.InvariantCulture);
            DeathStarCharge = float.Parse(json.deathstar_charge, CultureInfo.InvariantCulture);
            IsDeathStar = bool.Parse(json.is_deathstar);
            this.PlanetDistances = new Dictionary<Planet, float>();
        }

        public float GetDistance(Planet planet)
        {
            return (float)Math.Sqrt(Math.Pow(planet.PosX - PosX, 2) + Math.Pow(planet.PosY - PosY, 2));
        }
    }
}
