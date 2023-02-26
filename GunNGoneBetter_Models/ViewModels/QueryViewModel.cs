using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunNGoneBetter_Models.ViewModels
{
    public class QueryViewModel
    {
        public QueryHeader QueryHeader { get; set; }

        public IEnumerable<QueryDetail> QueryDetail { get; set; }
    }
}
