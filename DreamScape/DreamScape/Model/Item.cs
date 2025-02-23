using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DreamScape.Model;

namespace DreamScape.Model
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public Type Type { get; set; }
        public int RarityId { get; set; }
        public Rarity Rarity { get; set; }
        public int Power { get; set; }
        public int Speed { get; set; }
        public int Durability { get; set; }
        public string Magic { get; set; }

        // Additional properties to display related item names
        public string TypeName => Type?.Name;
        public string RarityName => Rarity?.Name;
    }
}
