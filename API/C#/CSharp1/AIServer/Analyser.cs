using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIServer
{
    public class Analyser
    {
        public UpdateContainer UpdateContainer { get; set; }
        public string MyName { get; set; }

        public Analyser(UpdateContainer container, string myName)
        {
            this.UpdateContainer = container;
            this.MyName = myName;
        }

        #region Info
        public List<Planet> MyPlanets
        {
            get
            {
                return this.UpdateContainer.Planets.Where(p => p.Owner == this.MyName).ToList();
            }
        }

        public List<Planet> NeutralPlanets
        {
            get
            {
                return this.UpdateContainer.Planets.Where(p => p.Owner == string.Empty).ToList();
            }
        }

        public List<Planet> EnemyPlanets
        {
            get
            {
                return this.UpdateContainer.Planets.Where(p => p.Owner != string.Empty && p.Owner != this.MyName).ToList();
            }
        }

        public List<Ship> MyShips  
        {
            get 
            {
                return this.UpdateContainer.Ships.Where(s => s.Owner == this.MyName).ToList();
            }
        }

        public List<Ship> EnemyShips
        {
            get
            {
                return this.UpdateContainer.Ships.Where(s => s.Owner != this.MyName).ToList();
            }
        }

        #endregion


    }
}
