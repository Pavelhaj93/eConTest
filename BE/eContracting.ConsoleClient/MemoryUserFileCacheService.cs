using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.ConsoleClient
{
    class MemoryUserFileCacheService : IUserFileCacheService
    {
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
