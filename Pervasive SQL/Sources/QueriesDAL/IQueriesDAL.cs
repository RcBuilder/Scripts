
using Entities;
using System.Collections.Generic;
using System.Data;

namespace QueriesDAL
{
    public interface IQueriesDAL
    {
        string Run(string query);
    }
}
