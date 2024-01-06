using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggergate;

namespace Talabat.Repository.Data
{
    public static class StoreContextSeed
    {
        public async static Task SeedAsync(StoreContext _dbContext)
        {
           if(_dbContext.ProductBrands.Count() == 0) { 
            var brandsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/brands.json");

            var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

            if (brands is not null && brands.Count > 0)
            {
                foreach (var brand in brands)
                {
                    _dbContext.Set<ProductBrand>().Add(brand);
                }
                await _dbContext.SaveChangesAsync();
            }
        }


            if (_dbContext.ProductCategories.Count() == 0)
            {
                var categoryData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/categories.json");

                var categoires = JsonSerializer.Deserialize<List<ProductCategory>>(categoryData);

                if (categoires is not null && categoires.Count() > 0)
                {
                    foreach (var category in categoires)
                    {
                        _dbContext.Set<ProductCategory>().Add(category);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }


            if(_dbContext.Products.Count() == 0)
            {
                var productData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/products.json");

                var products=JsonSerializer.Deserialize<List<Product>>(productData);

                if (products is not null && products.Count() > 0)
                {
                    foreach (var product in products)
                    {
                        _dbContext.Set<Product>().Add(product);
                    }
                    await _dbContext.SaveChangesAsync();
                }

            }


            if (_dbContext.DeliveryMethods.Count() == 0)
            {
                var deliveryMethodData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/delivery.json");

                var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodData);

                if (methods is not null && methods.Count() > 0)
                {
                    foreach (var method in methods)
                    {
                        _dbContext.Set<DeliveryMethod>().Add(method);
                    }
                    await _dbContext.SaveChangesAsync();
                }

            }

        }
    }
}
