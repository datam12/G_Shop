using System;
using Shop.Domain.DTO;
using Shop.Repository;
namespace Shop.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            UserRepository userRep = new UserRepository();
            

            var user = userRep.Get(1);
            Console.WriteLine(user.Username);

            var users = userRep.Load(null);
            foreach (var u in users)
            {
                Console.WriteLine(u.Username);
            }

            User test = new User();
            test.ID = 4;
            test.Username = "niiika";
            test.Password = "fiemfmei";
            //userRep.Insert(test);

            userRep.Update(test);

            userRep.Delete(1);

            ProductRepository productRep = new ProductRepository();

            var prod = productRep.Get("Vodka");

            foreach (var item in prod)
            {
                Console.WriteLine(item.ProductName+ ", " + item.UnitPrice);
            }

            Product product = new Product()
            {
                ProductId = 22,
                UnitPrice = 4M
            };

            productRep.Update(product);

        }
    }
}
