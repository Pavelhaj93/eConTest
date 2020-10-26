using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.ConsoleClient
{
    class MemoryCacheService : ICache
    {
        public void AddToPersist<T>(string key, T data, TimeSpan interval)
        {
            throw new NotImplementedException();
        }

        public void AddToRequest<T>(string key, T data)
        {
            throw new NotImplementedException();
        }

        public void AddToSession<T>(string key, T data)
        {
            throw new NotImplementedException();
        }

        public T GetFromPersist<T>(string key)
        {
            throw new NotImplementedException();
        }

        public T GetFromRequest<T>(string key)
        {
            throw new NotImplementedException();
        }

        public T GetFromSession<T>(string key)
        {
            throw new NotImplementedException();
        }
    }
}
