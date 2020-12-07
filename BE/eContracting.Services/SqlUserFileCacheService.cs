using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Storage;

namespace eContracting.Services
{
    public class SqlUserFileCacheService : IUserFileCacheService
    {
        protected readonly DatabaseContext Database;

        public SqlUserFileCacheService()
        {
            this.Database = new DatabaseContext();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void Set<T>(string key, T data)
        {
            throw new NotImplementedException();
        }
    }
}
