using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Domain.DTO;

namespace Shop.Repository
{
    public class ProductRepository : BaseRepository<Product>
    {
        public bool ProductQuantity(string ProductName)
        {
            DataTable table = _database.GetTable($"select productid from products where productname = '{ProductName}'");

            if (table.Rows.Count == 0)
            {
                return false;
            }
            else
                return true;
        }

        public List<Product> Get(string name)
        {
            DataTable table = _database.GetTable($"select * from products where productname = '{name}'");
            
            List<Product> list = new List<Product>();
            if (table.Rows.Count == 0) throw new ArgumentNullException("Product does not exists");

            DataRow row;

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    row = table.Rows[i];
                    Product product = new Product();
                    product.ProductId = Convert.ToInt32(row["productid"]);
                    product.ProductName = Convert.ToString(row["productname"]);
                    product.Description = Convert.ToString(row["description"]);
                    product.CategoryId = Convert.ToInt32(row["categoryid"]);
                    product.SupplierId = Convert.ToInt32(row["supplierid"]);
                    product.UnitPrice = Convert.ToDecimal(row["unitprice"]);
                    list.Add(product);
                }

            return list;
        }

        public override void Delete(int id)
        {
            var parameter = new SqlParameter { ParameterName = "@productid", Value = id };
            _database.ExecuteNonQuerry("DeleteProduct_sp", CommandType.StoredProcedure, parameter);
        }

        public override void Insert(Product product)
        {
            var parameters = new SqlParameter[]
            {
            new SqlParameter { ParameterName = "@productname", Value = product.ProductName},
            new SqlParameter { ParameterName = "@productid", Value = product.ProductId},
            new SqlParameter { ParameterName = "@description", Value = product.Description },
            new SqlParameter { ParameterName = "@categoryID", Value = product.CategoryId },
            new SqlParameter { ParameterName = "@supplierID", Value = product.SupplierId },
            new SqlParameter { ParameterName = "@unitPrice", Value = product.UnitPrice }
            };
            _database.ExecuteNonQuerry("InsertProduct_sp", CommandType.StoredProcedure, parameters);
        }

        public override void Update(Product product)
        {
            var parameters = new SqlParameter[]
            {
            new SqlParameter { ParameterName = "@productid", Value = product.ProductId},
            new SqlParameter { ParameterName = "@description", Value = product.Description },
            new SqlParameter { ParameterName = "@categoryid", Value = product.CategoryId },
            new SqlParameter { ParameterName = "@unitPrice", Value = product.UnitPrice }
            };
            _database.ExecuteNonQuerry("UpdateProduct_sp", CommandType.StoredProcedure, parameters);
        }
    }
}
