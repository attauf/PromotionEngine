namespace PromotionEngine.JsonObjects
{
    using Newtonsoft.Json;

    /// <summary>
    /// 
    /// </summary>
    public partial class UnitPriceObject
    {
        /// <summary>
        /// Gets or sets the products.
        /// </summary>
        /// <value>
        /// The products.
        /// </value>
        [JsonProperty("Products")]
        public Product[] Products { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class Product
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>
        /// The price.
        /// </value>
        [JsonProperty("Price")]
        public long Price { get; set; }
    }
}

