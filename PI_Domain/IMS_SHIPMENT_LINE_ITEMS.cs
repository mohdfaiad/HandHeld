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
    
    public partial class IMS_SHIPMENT_LINE_ITEMS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IMS_SHIPMENT_LINE_ITEMS()
        {
            this.IMS_SHIPMENT_ITEMS = new HashSet<IMS_SHIPMENT_ITEMS>();
        }
    
        public int ORIGREC { get; set; }
        public string ORIGSTS { get; set; }
        public int SHIPMENTLINEITEMID { get; set; }
        public int SHIPMENTID { get; set; }
        public Nullable<int> ORDERLINEITEMID { get; set; }
        public Nullable<int> CONTAINERID { get; set; }
        public int QUANTITYSHIPPED { get; set; }
        public string LOTNUMBER { get; set; }
        public Nullable<System.DateTime> LOTEXPIRATION { get; set; }
        public int PRODUCTID { get; set; }
        public string PO_NUMBER { get; set; }
        public string LINE_ITEM_NUMBER { get; set; }
        public string PO_CUSTOMER_REF { get; set; }
    
        public virtual IMS_PO_LINE_ITEMS IMS_PO_LINE_ITEMS { get; set; }
        public virtual IMS_PRODUCTS IMS_PRODUCTS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_SHIPMENT_ITEMS> IMS_SHIPMENT_ITEMS { get; set; }
        public virtual IMS_SHIPMENTS IMS_SHIPMENTS { get; set; }
        public virtual IMS_SHIPMENT_CONTAINERS IMS_SHIPMENT_CONTAINERS { get; set; }
    }
}
