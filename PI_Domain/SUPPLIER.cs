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
    
    public partial class SUPPLIER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SUPPLIER()
        {
            this.IMS_PRODUCTS = new HashSet<IMS_PRODUCTS>();
            this.IMS_PRODUCTS1 = new HashSet<IMS_PRODUCTS>();
            this.IMS_SUPPLIER_CUSTOMERS = new HashSet<IMS_SUPPLIER_CUSTOMERS>();
        }
    
        public string SUPZIP { get; set; }
        public string SUPPST { get; set; }
        public string SUPPNAM { get; set; }
        public string SUPPCODE { get; set; }
        public string SUPPCITY { get; set; }
        public string SUPPADD_A { get; set; }
        public string SUPPADD { get; set; }
        public string ORIGSTS { get; set; }
        public int ORIGREC { get; set; }
        public string JJCODE { get; set; }
        public string COUNTRY { get; set; }
        public string STATUS { get; set; }
        public string DESCRIPTION { get; set; }
        public string CURRENCYTYPE { get; set; }
        public string SUPPID { get; set; }
        public string COUNTRYCODE { get; set; }
        public string IS_EDITABLE { get; set; }
        public string BLOCKOUTBOUND { get; set; }
        public string QC_FAIL_RESOLUTION { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_PRODUCTS> IMS_PRODUCTS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_PRODUCTS> IMS_PRODUCTS1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMS_SUPPLIER_CUSTOMERS> IMS_SUPPLIER_CUSTOMERS { get; set; }
    }
}
