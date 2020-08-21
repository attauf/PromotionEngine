namespace PromotionEngineTests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PromotionEngine;

    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public class CalculatePriceTests
    {
        /// <summary>
        /// Calculates the price with null data.
        /// </summary>
        [TestMethod]
        public void CalculatePriceWithNullData()
        {
            var response = Program.CalculatePrice(null);
            response.Should().NotBeNull("because proper AmountResponse object should be returned");
            response.Status.Should().Be(StatusEnum.NoProductInCart, "because products list should be available in cart");
            response.Amount.Should().Be(0, "Amount should be zero, as no products are listed in cart");
        }

        /// <summary>
        /// Calculates the price with empty data.
        /// </summary>
        [TestMethod]
        public void CalculatePriceWithEmptyData()
        {
            var response = Program.CalculatePrice(new List<char> { });
            response.Should().NotBeNull("because proper AmountResponse object should be returned");
            response.Status.Should().Be(StatusEnum.NoProductInCart, "because products list should be available in cart");
            response.Amount.Should().Be(0, "Amount should be zero, as no products are listed in cart");
        }

        /// <summary>
        /// Calculates the price with valid data.
        /// </summary>
        [TestMethod]
        public void CalculatePriceWithValidData()
        {
            var response = Program.CalculatePrice(new List<char> { 'A' });
            response.Should().NotBeNull("because proper AmountResponse object should be returned");
            response.Status.Should().Be(StatusEnum.Success, "because valid data was provided");
            response.Amount.Should().Be(50, "Amount should not be zero, as valid data is provided");
        }

        /// <summary>
        /// Calculates the price with no unit price for a product.
        /// </summary>
        [TestMethod]
        public void CalculatePriceWithNoUnitPriceForProduct()
        {
            var response = Program.CalculatePrice(new List<char> { 'E'});
            ConfigurationManager.AppSettings.Clear();
            response.Should().NotBeNull("because proper AmountResponse object should be returned");
            response.Status.Should().Be(StatusEnum.UnitPriceMissingForProduct, "because the config key doesn't has unit prices for the provided product");
            response.Amount.Should().Be(0, "Amount should be zero, as no unit prices are configured for the provided product");
        } 
    }
}
