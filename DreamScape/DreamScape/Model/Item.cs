using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamScape.Model
{
    class Item
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
    }
}
