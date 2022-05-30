using Shop.Repository;
using Xunit;

namespace Product
{
    public class UnitTest1
    {
        private ProductRepository _productRepository = new ProductRepository();

        [Fact]
        public void Test1()
        {
            var result = _productRepository.ProductQuantity("Milk");
            Assert.True(result, "Product doesnot exist");
        }
    }
}
