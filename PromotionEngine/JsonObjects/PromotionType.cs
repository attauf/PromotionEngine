namespace PromotionEngine.JsonObjects
{
    using Newtonsoft.Json;

    /// <summary>
    /// 
    /// </summary>
    public partial class PromotionTypeObject
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("PromotionTypes")]
        public PromotionType PromotionTypes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("NumberOfPromotionTypesCanApply")]
        public long NumberOfPromotionTypesCanApply { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class PromotionType
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("Types")]
        public TypeElement[] Types { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class TypeElement
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("Promotion")]
        public string Promotion { get; set; }
    }
}

