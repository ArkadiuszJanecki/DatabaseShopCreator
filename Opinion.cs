//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Database_Shop_Creator_Optimalized
{
    using System;
    using System.Collections.Generic;
    
    public partial class Opinion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Opinion()
        {
            this.Product = new HashSet<Product>();
            this.Product11 = new HashSet<Product>();
        }
    
        public int ID_Opinion { get; set; }
        public byte Rating { get; set; }
        public string Comment { get; set; }
        public int ID_Person { get; set; }
        public Nullable<int> ID_Product { get; set; }
    
        public virtual Person Person { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Product { get; set; }
        public virtual Product Product1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Product11 { get; set; }
    }
}
