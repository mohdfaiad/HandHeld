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
    
    public partial class LOCATION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LOCATION()
        {
            this.IMS_ITEMS = new HashSet<IMS_ITEMS>();
        }
    
        public Nullable<int> SUBLOCATIONINFO_ID { get; set; }
        public string SERVGRP { get; set; }
        public Nullable<int> ROOM_ID { get; set; }
        public string PARENT_LOCATION_CODE { get; set; }
        public string ORIGSTS { get; set; }
        public int ORIGREC { get; set; }
        public string LONGNAME { get; set; }
        public string LONGCODE { get; set; }
        public string LOCATIONCODE { get; set; }
        public Nullable<int> LOCATION_TYPE_ID { get; set; }
        public string LOCATION_NAME { get; set; }
        public string LOCATION1 { get; set; }
        public string IS_STORABLE { get; set; }
        public string IS_GXP { get; set; }
        public string DISPLAY_LOCATIONCODE { get; set; }
        public string DESCRIPTION { get; set; }
        public string DEPT { get; set; }
        public string CONDITION { get; set; }
        public string TO_BE_COUNTED { get; set; }
        public Nullable<int> MIN_INTERVAL_COUNT { get; set; }
        public Nullable<System.DateTime> LAST_COUNT_DATE { get; set; }
        public Nullable<int> MAX_INTERVAL_COUNT { get; set; }
        public string COUNT_INTERVAL_UNIT { get; set; }
        public string SEALED { get; set; }
        public Nullable<int> CAPACITY { get; set; }
        public string HOLDING_MATERIAL { get; set; }
        public Nullable<int> SORTER { get; set; }
        public string HAS_RFID_COVERAGE { get; set; }
        public string FULLNAME { get; set; }
        public string INTRANSIT { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_ITEMS> IMS_ITEMS { get; set; }
        public virtual ROOM ROOM { get; set; }
    }
}
