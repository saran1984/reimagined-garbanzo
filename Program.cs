using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PromotionEngineApp
{   
    public class Program
    {
        public static void Main(string[] args)
        {
            bool isSKU1 = false;
            bool isSKU2 = false;
            //create list of promotions
            //we need to add information about Product's count
            Dictionary<String, int> d1 = new Dictionary<String, int>();
            d1.Add("A", 3);
            Dictionary<String, int> d2 = new Dictionary<String, int>();
            d2.Add("B", 2);
            Dictionary<String, int> d3 = new Dictionary<String, int>();
            d3.Add("C", 1);
            d3.Add("D", 1);

            List<Promotion> promotions = new List<Promotion>()
            {
                new Promotion(1, d1, 130, 50),
                new Promotion(2, d2, 45, 30),
                new Promotion(3, d3, 30, 20)
            };

            //create orders
            List<Order> orders = new List<Order>();
            Order order1 = new Order(1, new List<Product>() { new Product("A"), new Product("B"), new Product("C") });
            Order order2 = new Order(2, new List<Product>() { new Product("A"), new Product("A"), new Product("A"), new Product("A"), new Product("A"), new Product("B"), new Product("B"), new Product("B"), new Product("B"), new Product("B"), new Product("C") });
            Order order3 = new Order(3, new List<Product>() { new Product("A"), new Product("A"), new Product("A"), new Product("B"), new Product("B"), new Product("B"), new Product("B"), new Product("B"), new Product("C"), new Product("D") });
            orders.AddRange(new Order[] { order1, order2, order3 });
            //check if order meets promotion
            foreach (Order ord in orders)
            {
                int isACount = 0;
                int isBCount = 0;
                int isDCount = 0;
                foreach (Product p in ord.Products)
                {
                    if (p.Id == "A")
                        isACount += 1;
                }
                if (isACount > 2)
                {
                    foreach (Product p in ord.Products)
                    {
                        if (p.Id == "B")
                            isBCount += 1;
                    }
                }

                foreach (Product p in ord.Products)
                {
                    if (p.Id == "C" || p.Id == "D")
                    {
                        isDCount += 1;
                    }
                }

                if (isACount > 1 && isBCount > 1)
                {
                    isSKU1 = true;
                }

                if (isDCount > 1)
                {
                    isSKU2 = true;
                }

                List<decimal> promoprices = promotions
                    .Select(promo => PromotionChecker.GetTotalPrice(ord, promo, isSKU1, isSKU2))
                    .ToList();
                decimal origprice = ord.Products.Sum(x => x.Price);
                decimal promoprice = promoprices.Sum();
                if (promoprice == 0)
                    promoprice = origprice;
                Console.WriteLine("OrderID: " + ord.OrderID + " Total: " + promoprice.ToString("0.00"));
            }
            Console.WriteLine("Press any key to exit.");
            Console.Read();
        }

        // Define other methods and classes here
        public class Product
        {

            public string Id { get; set; }
            public decimal Price { get; set; }


            public Product(string id)
            {
                this.Id = id;
                switch (id)
                {
                    case "A":
                        this.Price = 50m;

                        break;
                    case "B":
                        this.Price = 30m;

                        break;
                    case "C":
                        this.Price = 20m;

                        break;
                    case "D":
                        this.Price = 15m;
                        break;
                }
            }
        }

        public class Promotion
        {
            public int PromotionID  { get; set; }
            public Dictionary<string, int> ProductInfo { get; set; }
            public decimal PromoPrice  { get; set; }
            public decimal ActualPrice { get; set; }

            public Promotion(int _promID, Dictionary<string, int> _prodInfo, decimal _pp, decimal _ap)
            {
                this.PromotionID = _promID;
                this.ProductInfo = _prodInfo;
                this.PromoPrice = _pp;
                this.ActualPrice = _ap;
            }
        }

        public class Order
        {
            public int OrderID  { get; set; }
            public List<Product> Products { get; set; }

            public Order(int _oid, List<Product> _prods)
            {
                this.OrderID = _oid;
                this.Products = _prods;
            }
        }

        public static class PromotionChecker
        {
            //returns PromotionID and count of promotions
            public static decimal GetTotalPrice(Order ord, Promotion prom, bool isSKU1, bool isSKU2)
            {
                decimal d = 0M;

                //get count of promoted products in order
                var copp = ord.Products
                    .GroupBy(x => x.Id)
                    .Where(grp => prom.ProductInfo.Any(y => grp.Key == y.Key && grp.Count() >= y.Value))
                    .Select(grp => grp.Count())
                    .Sum();

                //get count of promoted products from promotion
                int ppc = prom.ProductInfo.Sum(kvp => kvp.Value);

                while (copp >= ppc)
                {
                    d += prom.PromoPrice;
                    copp -= ppc;
                }

                if (copp > 0)
                {
                    if (isSKU1 == true && isSKU2 == true)
                        d += copp * 30;
                    else if (isSKU1 == true)
                        d += copp * prom.ActualPrice;
                }
                return d;
            }
        }
    }
}


