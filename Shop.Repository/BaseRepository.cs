using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseHelper;
using Shop.Domain.DTO;

namespace Shop.Repository
{
    public abstract class BaseRepository<T>

    {
        private const string ConnectionString = @"server = .\SQLEXPRESS01; database = ShopMarket; integrated security = true";

        protected SqlClientDatabase _database;

        public BaseRepository()
        {
            _database = new SqlClientDatabase(ConnectionString);
        }

        public abstract void Delete(int id);

        public abstract void Insert(T insertItem);

        public abstract void Update(T updateItem);
  
    }
}
