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
    
    public partial class DEPARTMENT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DEPARTMENT()
        {
            this.BUILDINGS = new HashSet<BUILDING>();
            this.IMS_COST_CENTERS = new HashSet<IMS_COST_CENTERS>();
        }
    
        public string ZIP { get; set; }
        public string URL { get; set; }
        public string STATE { get; set; }
        public string SHARESDB { get; set; }
        public string RASCLIENTID { get; set; }
        public string PHONE2 { get; set; }
        public string PHONE1 { get; set; }
        public string ORIGSTS { get; set; }
        public int ORIGREC { get; set; }
        public string NAME { get; set; }
        public string MANAGER { get; set; }
        public string KNOWN_AS { get; set; }
        public string FAX { get; set; }
        public string DEPTCODE { get; set; }
        public string DEPT { get; set; }
        public string COUNTRY { get; set; }
        public string CITY { get; set; }
        public string ADDRESS2 { get; set; }
        public string ADDRESS1 { get; set; }
        public string ACCOUNT_NO { get; set; }
        public string SHORTDESCRIPTION { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BUILDING> BUILDINGS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_COST_CENTERS> IMS_COST_CENTERS { get; set; }
    }
}
