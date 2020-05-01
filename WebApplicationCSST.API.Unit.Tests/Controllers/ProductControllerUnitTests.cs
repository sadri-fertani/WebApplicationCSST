using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationCSST.API.Controllers;
using WebApplicationCSST.Service;
using WebApplicationCSST.Service.Models;

namespace WebApplicationCSST.API.Unit.Tests
{
    public class ProductControllerUnitTests
    {
        private Mock<ILogger<ProductController>> _mockLogger;
        private Mock<IProductService> _mockProductService;

        [OneTimeSetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<ProductController>>();
            _mockProductService = new Mock<IProductService>();
        }

        [Test]
        public async Task GetOne_WhenCalledWithId_ReturnProductModelResult_Async()
        {
            // Arrange
            ProductModel iPhoneXr = new ProductModel
            {
                Id = 219,
                ProductName = "iPhone XR",
                ProductDetails = null            
            };

            _mockProductService
                .Setup(r => r.GetProduct(219)).Returns(Task.FromResult(iPhoneXr));

            var controller = new ProductController(
                _mockLogger.Object,
                _mockProductService.Object);

            // Act
            var actionResult = await controller.GetOne(219);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsAssignableFrom(typeof(ActionResult<ProductModel>), actionResult);

            Assert.That(actionResult.Value, Is.EqualTo(iPhoneXr));
        }

        [Test]
        public async Task GetOne_WhenCalledWithWrongId_ReturnNotFoundResult_Async()
        {
            var controller = new ProductController(
                _mockLogger.Object,
                _mockProductService.Object);

            // Act
            var actionResult = await controller.GetOne(0);

            // Assert
            Assert.IsNotNull(actionResult);

            Assert.AreEqual((actionResult.Result as ObjectResult).StatusCode, StatusCodes.Status404NotFound);
        }

        [Test]
        public async Task GetAll_ReturnListProductModelResult_Async()
        {
            // Arrange
            IList<ProductModel> phones = new List<ProductModel>();
            phones.Add(new ProductModel { Id = 1, ProductName = "phone n° 1" });
            phones.Add(new ProductModel { Id = 2, ProductName = "phone n° 2" });
            phones.Add(new ProductModel { Id = 3, ProductName = "phone n° 3" });
            phones.Add(new ProductModel { Id = 4, ProductName = "phone n° 4" });

            _mockProductService
                .Setup(r => r.GetProducts()).Returns(Task.FromResult(phones.AsEnumerable()));

            var controller = new ProductController(
                _mockLogger.Object,
                _mockProductService.Object);

            // Act
            var result = await controller.GetAll();

            // Assert
            var actionResult = result;
            Assert.IsNotNull(actionResult);
            Assert.IsAssignableFrom(typeof(ActionResult<List<ProductModel>>), actionResult);

            Assert.That(actionResult.Value, Is.Not.Null);
        }

        [Test]
        public async Task CreateOne_WhenCalled_ReturnProductModelResult_Async()
        {
            // Arrange
            ProductModel dellBeforeInsert = new ProductModel
            {
                Id = 0,
                ProductName = "Dell",
                ProductDetails = null
            };

            // Arrange
            ProductModel dellAfterInsert = new ProductModel
            {
                Id = (new Random()).Next(1, 100),
                ProductName = "Dell",
                ProductDetails = null
            };

            _mockProductService
                .Setup(r => r.InsertProduct(dellBeforeInsert)).Returns(Task.FromResult(dellAfterInsert));

            var controller = new ProductController(
                _mockLogger.Object,
                _mockProductService.Object);

            // Act
            var result = await controller.Post(dellBeforeInsert);

            // Assert
            var actionResult = result;
            Assert.IsNotNull(actionResult);
            Assert.That(((actionResult.Result as CreatedAtActionResult).Value as ProductModel).Id, Is.Not.EqualTo(0));
        }

        [Test]
        public async Task UpdateOne_WhenCalled_ReturnNoContentResult_Async()
        {
            // Arrange
            ProductModel dellBeforeUpdate = new ProductModel
            {
                Id = 20,
                ProductName = "Dell nura",
                ProductDetails = null
            };

            // Arrange
            ProductModel dellAfterUpdate = new ProductModel
            {
                Id = 20,
                ProductName = "Dell nura updated",
                ProductDetails = null
            };

            _mockProductService
                .Setup(r => r.GetProduct(dellBeforeUpdate.Id)).Returns(Task.FromResult(dellBeforeUpdate));
            _mockProductService
                .Setup(r => r.UpdateProduct(dellBeforeUpdate)).Returns(Task.FromResult(dellAfterUpdate));

            var controller = new ProductController(
                _mockLogger.Object,
                _mockProductService.Object);

            // Act
            var actionResult = await controller.Put(dellBeforeUpdate);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual((actionResult as StatusCodeResult).StatusCode, StatusCodes.Status204NoContent);
        }

        [Test]
        public async Task DeleteOne_WhenCalledWithId_ReturnProductModelResult_Async()
        {
            // Arrange
            ProductModel dellToDelete = new ProductModel
            {
                Id = 50,
                ProductName = "Dell nura",
                ProductDetails = null
            };

            var controller = new ProductController(
                _mockLogger.Object,
                _mockProductService.Object);

            _mockProductService
                .Setup(r => r.GetProduct(dellToDelete.Id)).Returns(Task.FromResult(dellToDelete));
            _mockProductService
                .Setup(r => r.DeleteProduct(dellToDelete.Id)).Verifiable();

            // Act
            var actionResult = await controller.Delete(50);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsAssignableFrom(typeof(ActionResult<ProductModel>), actionResult);

            Assert.That(actionResult.Value, Is.EqualTo(dellToDelete));
        }
    }
}