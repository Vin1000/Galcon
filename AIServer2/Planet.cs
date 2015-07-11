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
                return PlanetDistances.Keys.OrderBy(p => this.GetDistance(p)).Where(p => p.IsDeathStar == false).Take(number).ToList();
            }
            return PlanetDistances.Keys.Where(p => p.Id != this.Id && p.IsDeathStar == false).OrderBy(p => this.GetDistance(p)).Take(number).ToList();
        }

        public List<Planet> GetMyClosestPlanets(string owner)
        {
            return PlanetDistances.Keys.Where(p => p.Id != this.Id && p.Owner == owner).OrderBy(p => this.GetDistance(p)).ToList();
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

        private int _turnsPossessed = 1;

        private const int DeathStarAlmostReady = 6;

        public bool ReadyToAttackNextTurn
        {
            get
            {
                return this._turnsPossessed >= DeathStarAlmostReady;
            }
        }

        public void Update(string newOwner, int shipCount, float deathStarCharge)
        {
            if (this.IsDeathStar)
            {
                this.DeathStarCharge = deathStarCharge;
                if (this.Owner == newOwner)
                {
                    if (newOwner != String.Empty)
                    {
                        this._turnsPossessed++;
                    }
                }
                else
                {
                    this._turnsPossessed = 1;
                }
                if (this._turnsPossessed >= 6 && this.DeathStarCharge <= (float)1.5/(float)7.0)
                {
                    this._turnsPossessed = 1;
                }
            }
            this.Owner = newOwner;
            this.ShipCount = shipCount;
        }
    }
}
