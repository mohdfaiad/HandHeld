using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PI_Abstract;


namespace PI_Models
{
    public class Location : ISelect, ISkip, IMoveLocation
    {
              
        public string locationCode { get; set; }
        public string locationName { get; set; }
        public string Site { get; set; }
        public List<Item> items { get; set; }
        public List<CostCenter> costcenters { get; set; }
        private bool select;
        private bool skip = false;
        private bool movelocationonnly = false;
      

        public bool Select
        {
            get
            {
                return select;
            }
            set
            {
                select = value;
            }
        }
        public bool Skip
        {
            get
            {
                return skip;
            }
            set
            {
                skip = value;
            }
        }
        public bool MoveItemsOnly
        {
            get
            {
                return movelocationonnly;
            }
            set
            {
                movelocationonnly = value;
            }
        }
        
       
    }
}
