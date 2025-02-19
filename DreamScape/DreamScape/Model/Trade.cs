﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamScape.Model
{
    class Trade
    {
        public int Id { get; set; }
        public int SellerId { get; set; }
        public User Seller { get; set; }
        public int BuyerId { get; set; }
        public User Buyer { get; set; }
        public ObservableCollection<int> ItemId { get; set; }
        public ObservableCollection<Item> Item { get; set; }
        public int StatusId { get; set; }
    }
}
