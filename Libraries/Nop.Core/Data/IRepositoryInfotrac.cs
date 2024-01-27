using System.Collections.Generic;
using System.Linq;

namespace Nop.Core.Data
{

    public partial interface IRepositoryInfotrac
    {
       IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters);
    }
}
