using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;

namespace Nop.Data
{
    /// <summary>
    /// Entity Framework repository
    /// </summary>
    public partial class EfRepositoryInfotrac : IRepositoryInfotrac
    {

        private readonly IDbContextInfotrac _context;

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public EfRepositoryInfotrac(IDbContextInfotrac context)
        {
            this._context = context;


            
        }


        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return this._context.SqlQuery<TElement>(sql, parameters);
        }

        #endregion


    }
}