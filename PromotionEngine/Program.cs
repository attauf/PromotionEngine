namespace PromotionEngine
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using PromotionEngine.JsonObjects;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            List<char> productsInCart = new List<char> { 'A', 'A', 'A', 'A', 'A', 'B', 'B', 'B', 'B', 'B', 'C' };
            var totalPrice = CalculatePrice(productsInCart);
        }

        /// <summary>
        /// Calculates the price.
        /// </summary>
        /// <param name="productsInCart">The products in cart.</param>
        /// <returns></returns>
        public static AmountResponse CalculatePrice(List<char> productsInCart)
        {
            try
            {
                if (productsInCart == null || !productsInCart.Any())
                {
                    return new AmountResponse { Status = StatusEnum.NoProductInCart };
                }

                var unitPrices = ConfigurationManager.AppSettings["UnitPrices"];
                if (string.IsNullOrWhiteSpace(unitPrices))
                {
                    return new AmountResponse { Status = StatusEnum.UnitPricesMissing };
                }

                var deserializedUnitPrices = JsonConvert.DeserializeObject<UnitPriceObject>(unitPrices);
                if (deserializedUnitPrices==null || deserializedUnitPrices.Products==null || !deserializedUnitPrices.Products.Any())
                {
                    return new AmountResponse { Status = StatusEnum.UnitPricesMissing };
                }

                var items = productsInCart.GroupBy(x => x).Select(g => new UniqueProduct { Product = g.Key, Count = g.Count() }).OrderBy(x => x.Product).ToList();
                var promotionTypes = ConfigurationManager.AppSettings["PromotionTypes"];
                
                if (!string.IsNullOrWhiteSpace(promotionTypes))
                {
                    var deserializedPromotions = JsonConvert.DeserializeObject<PromotionTypeObject>(promotionTypes);
                    if (deserializedPromotions?.PromotionTypes != null && deserializedPromotions.PromotionTypes.Enabled)
                    {
                        var promotions = deserializedPromotions.PromotionTypes.Types;
                        if (promotions==null || !promotions.Any())
                        {
                            return GetAmountWithoutPromotion(items, deserializedUnitPrices);
                        }
                        else
                        {
                            long amount = 0;
                            return new AmountResponse { Status = StatusEnum.Success, Amount = GetAmountWithPromotion(ref items, ref amount, deserializedUnitPrices, deserializedPromotions) };
                        }             
                    }
                    else
                    {
                        return GetAmountWithoutPromotion(items, deserializedUnitPrices);
                    }
                }

                return GetAmountWithoutPromotion(items, deserializedUnitPrices);
            }
            catch (Exception ex)
            {
                return new AmountResponse { Status = StatusEnum.Fail };
            }            
        }

        /// <summary>
        /// Gets the amount without promotion.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="deserializedUnitPrices">The deserialized unit prices.</param>
        /// <returns></returns>
        public static AmountResponse GetAmountWithoutPromotion(List<UniqueProduct> items, UnitPriceObject deserializedUnitPrices)
        {
            long amountWithoutPromotion = 0;
            foreach (var item in items)
            {
                var unitPrice = deserializedUnitPrices.Products.Where(x => x.Equals(item.Product)).FirstOrDefault();
                if (unitPrice != null || unitPrice.Price != 0)
                {
                    amountWithoutPromotion += item.Count * unitPrice.Price;
                }
                else
                {
                    return new AmountResponse { Status = StatusEnum.UnitPriceMissingForProduct };
                }
            }

            return new AmountResponse { Status = StatusEnum.Success, Amount = amountWithoutPromotion };
        }

        /// <summary>
        /// Gets the amount with promotion.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="deserializedUnitPrices">The deserialized unit prices.</param>
        /// <param name="deserializedPromotions">The deserialized promotions.</param>
        /// <returns></returns>
        public static long GetAmountWithPromotion(ref List<UniqueProduct> items, ref long amount, UnitPriceObject deserializedUnitPrices, PromotionTypeObject deserializedPromotions)
        {
            var promotions = deserializedPromotions.PromotionTypes.Types;
            foreach (var item in items)
            {
                var allPromotionsForProduct = promotions.Where(x => x.IsActive && x.Promotion.Contains(item.Product.ToString())).ToList();
                if (allPromotionsForProduct != null && allPromotionsForProduct.Any())
                {
                    Dictionary<string, int> dict = new Dictionary<string, int>();
                    List<UniqueProduct> uniqueProducts = new List<UniqueProduct>();
                    int numberOfAppliedPromotions = 0;
                    foreach (var promotionForProduct in allPromotionsForProduct)
                    {
                        if (numberOfAppliedPromotions < deserializedPromotions.NumberOfPromotionTypesCanApply)
                        {
                            var productPromotionCount = promotionForProduct.Promotion.Count(x => x.Equals(item.Product));
                            if (item.Count > productPromotionCount)
                            {
                                amount += Convert.ToInt64(promotionForProduct.Promotion.Split('=')[1]);
                                numberOfAppliedPromotions++;
                                if (uniqueProducts.Exists(x => x.Product.Equals(item.Product.ToString())))
                                {
                                    var oldCount = uniqueProducts.FirstOrDefault(x => x.Product.Equals(item.Product.ToString())).Count;
                                    uniqueProducts.FirstOrDefault(x => x.Product.Equals(item.Product.ToString())).Count += oldCount + item.Count - productPromotionCount;
                                }
                                else
                                {
                                    uniqueProducts.Add(new UniqueProduct { Product = item.Product, Count = item.Count - productPromotionCount });
                                }
                            }
                            else
                            {
                                amount += deserializedUnitPrices.Products.FirstOrDefault(x => x.Name.Equals(item.Product.ToString())).Price * item.Count;
                            }                         
                        }
                    }
                    if (uniqueProducts.Count > 0)
                    {
                        amount += GetAmountWithPromotion(ref uniqueProducts, ref amount, deserializedUnitPrices, deserializedPromotions);
                    }
                }
                else
                {
                    amount += deserializedUnitPrices.Products.FirstOrDefault(x => x.Name.Equals(item.Product.ToString())).Price * item.Count;
                }
            }

            return amount;
        }
    }
}
