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
    
    public partial class METADATA_LOOKUP_VALUES
    {
        public string VALUE { get; set; }
        public string TEXT { get; set; }
        public string ORIGSTS { get; set; }
        public int ORIGREC { get; set; }
        public string LOOKUP_NAME { get; set; }
        public int SORTER { get; set; }
    
        public virtual METADATA_LOOKUPS METADATA_LOOKUPS { get; set; }
    }
}
