//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PI_Domain
{
    using System;
    using System.Collections.Generic;
    
    public partial class IMS_SHIPMENT_ITEMS
    {
        public int ORIGREC { get; set; }
        public string ORIGSTS { get; set; }
        public int SHIPMENTITEMID { get; set; }
        public Nullable<int> CONTAINERID { get; set; }
        public int SHIPMENTLINEITEMID { get; set; }
        public string TAGREADSTATUS { get; set; }
        public Nullable<System.DateTime> TAGREADTIME { get; set; }
        public int ITEMID { get; set; }
    
        public virtual IMS_ITEMS IMS_ITEMS { get; set; }
        public virtual IMS_SHIPMENT_LINE_ITEMS IMS_SHIPMENT_LINE_ITEMS { get; set; }
        public virtual IMS_SHIPMENT_CONTAINERS IMS_SHIPMENT_CONTAINERS { get; set; }
    }
}
