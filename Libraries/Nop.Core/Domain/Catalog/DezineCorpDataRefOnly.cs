using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{
    public class DezineCorpDataRefOnly : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// Gets or sets the Old Page # 2012
        /// </summary>
        public string OldPage2012 { get; set; }

        /// <summary>
        /// Gets or sets the Net price 1
        /// </summary>
        public string Net1 { get; set; }

        /// <summary>
        /// Gets or sets the Net price 2
        /// </summary>
        public string Net2 { get; set; }

        /// <summary>
        /// Gets or sets the Net price 3
        /// </summary>
        public string Net3 { get; set; }

        /// <summary>
        /// Gets or sets the Net price 4
        /// </summary>
        public string Net4 { get; set; }

        /// <summary>
        /// Gets or sets the Net price 5
        /// </summary>
        public string Net5 { get; set; }

        /// <summary>
        /// Gets or sets the Net price 6
        /// </summary>
        public string Net6 { get; set; }

        /// <summary>
        /// Gets or sets the Net price 7
        /// </summary>
        public string Net7 { get; set; }

        /// <summary>
        /// Gets or sets the Net price 7
        /// </summary>
        public string Net8 { get; set; }

        /// <summary>
        /// Gets or sets the LOWEST INVOICE VALUE EQP MOQ
        /// </summary>
        public string LOWESTINVOICEVALUEEQPMOQ { get; set; }

        /// <summary>
        /// Gets or sets the Current EQP
        /// </summary>
        public string CurrentEQP { get; set; }

        /// <summary>
        /// Gets or sets the Current EQP less 5%
        /// </summary>
        public string CurrentEQPLess5PerCent { get; set; }

        /// <summary>
        /// Gets or sets the Change 2010 to 2011 EQP to EQP
        /// </summary>
        public string Change2010to2011EQPtoEQP { get; set; }

        /// <summary>
        /// Gets or sets the Country of Origin
        /// </summary>
        public string CountryofOrigin { get; set; }

        /// <summary>
        /// Gets or sets the HS Code
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// Gets or sets the Master Pack
        /// </summary>
        public string MasterPack { get; set; }

        /// <summary>
        /// Gets or sets the Lenght
        /// </summary>
        public string L { get; set; }

        /// <summary>
        /// Gets or sets the Widht
        /// </summary>
        public string W { get; set; }

        /// <summary>
        /// Gets or sets the Height
        /// </summary>
        public string H { get; set; }

        /// <summary>
        /// Gets or sets the Volume
        /// </summary>
        public string Volume { get; set; }

        /// <summary>
        /// Gets or sets the Freight Unit
        /// </summary>
        public string FreightUnit { get; set; }

        /// <summary>
        /// Gets or sets the  Date Revised
        /// </summary>
        public string DateRevised { get; set; }

        /// <summary>
        /// Gets or sets the Revised By
        /// </summary>
        public string RevisedBy { get; set; }

        /// <summary>
        /// Gets or sets the Internal Comments
        /// </summary>
        public string InternalComments { get; set; }

        /// <summary>
        /// Gets or sets the PPPC Notes
        /// </summary>
        public string PPPCNotes { get; set; }

        /// <summary>
        /// Gets or sets the Dezine Category
        /// </summary>
        public string DezineCategory { get; set; }

        /// <summary>
        /// Gets or sets the INFOtrac Import Result if Error
        /// </summary>
        public string INFOtracImportResultifError { get; set; }

    }
}
