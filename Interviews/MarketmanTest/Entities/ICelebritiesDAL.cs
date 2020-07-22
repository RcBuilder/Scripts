using System;
using System.Collections.Generic;

namespace Entities
{
    public interface ICelebritiesDAL
    {
        List<CelebrityCard> Get();
        int Save(IEnumerable<CelebrityCard> Cards);
        bool Delete(string CardId);
        bool Create(CelebrityCard Card);
        bool Update(CelebrityCard Card);
    }
}
