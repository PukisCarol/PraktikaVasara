using Moq;
using Praktika2025Vasara.Entities;
using Praktika2025Vasara.Services;
using Praktika2025Vasara.Store;

namespace Praktika2025Vasara.Tests
{
    [TestClass]
    public class ShortageServiceTests
    {
        private Mock<IFileStore<Shortage>> _mockStore;
        private ShortageService _shortageService;
        private List<Shortage> _shortages;

        [TestInitialize]
        public void Setup()
        {
            _mockStore = new Mock<IFileStore<Shortage>>();
            _shortages = new List<Shortage>();
            _mockStore.Setup(m => m.GetItems()).Returns(_shortages);
            _mockStore.Setup(m => m.SaveItems(It.IsAny<List<Shortage>>())).Callback<List<Shortage>>(items => _shortages = items);
            _shortageService = new ShortageService(_mockStore.Object);
        }

        [TestMethod]
        public void DoesShortageExists_SameTitleAndRoomWithHigherPriority_ReturnsTrue()
        {
            // Arrange
            var existingShortage = new Shortage("Paper", "ProductName", Room.Kitchen, Category.Other, 3, "user1");
            _shortages.Add(existingShortage);
            var newShortage = new Shortage("Paper", "ProductName", Room.Kitchen, Category.Other, 5, "user2");

            // Act
            var result = _shortageService.DoesShortageExists(newShortage);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DoesShortageExists_SameTitleAndRoomWithLowerPriority_ReturnsFalse()
        {
            // Arrange
            var existingShortage = new Shortage("Paper", "ProductName", Room.Kitchen, Category.Other, 5, "user1");
            _shortages.Add(existingShortage);
            var newShortage = new Shortage("Paper", "ProductName", Room.Kitchen, Category.Other, 3, "user2");

            // Act
            var result = _shortageService.DoesShortageExists(newShortage);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DoesShortageExists_DifferentTitleOrRoom_ReturnsFalse()
        {
            // Arrange
            var existingShortage = new Shortage("Paper", "ProductName", Room.Kitchen, Category.Other, 3, "user1");
            _shortages.Add(existingShortage);
            var newShortage = new Shortage("Soap", "ProductName2", Room.Bathroom, Category.Other, 5, "user2");

            // Act
            var result = _shortageService.DoesShortageExists(newShortage);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AddShortage_AddsNewShortageToStore()
        {
            // Arrange
            var newShortage = new Shortage("Paper", "ProductName", Room.Kitchen, Category.Other, 5, "user1");

            // Act
            _shortageService.AddShortage(newShortage);

            // Assert
            Assert.AreEqual(1, _shortages.Count);
            Assert.AreEqual(newShortage, _shortages[0]);
            _mockStore.Verify(m => m.SaveItems(It.IsAny<List<Shortage>>()), Times.Once());
        }

        [TestMethod]
        public void DeleteShortage_ValidUserAndShortage_ReturnsTrue()
        {
            // Arrange
            var shortage = new Shortage("Paper", "ProductName", Room.Kitchen, Category.Other, 5, "user1");
            _shortages.Add(shortage);

            // Act
            var result = _shortageService.DeleteShortage("Paper", Room.Kitchen, "user1", false);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, _shortages.Count);
            _mockStore.Verify(m => m.SaveItems(It.IsAny<List<Shortage>>()), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteShortage_ShortageNotFound_ThrowsException()
        {
            // Act
            _shortageService.DeleteShortage("Paper", Room.Kitchen, "user1", false);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteShortage_NonCreatorNonAdmin_ThrowsException()
        {
            // Arrange
            var shortage = new Shortage("Paper", "ProductName", Room.Kitchen, Category.Other, 5, "user1");
            _shortages.Add(shortage);

            // Act
            _shortageService.DeleteShortage("Paper", Room.Kitchen, "user2", false);
        }

        [TestMethod]
        public void DeleteShortage_AdminCanDeleteAnyShortage_ReturnsTrue()
        {
            // Arrange
            var shortage = new Shortage("Paper", "ProductName", Room.Kitchen, Category.Other, 5, "user1");
            _shortages.Add(shortage);

            // Act
            var result = _shortageService.DeleteShortage("Paper", Room.Kitchen, "user2", true);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, _shortages.Count);
            _mockStore.Verify(m => m.SaveItems(It.IsAny<List<Shortage>>()), Times.Once());
        }

        [TestMethod]
        public void FindShortages_NonAdmin_ReturnsOnlyUserShortages()
        {
            // Arrange
            var userShortage = new Shortage("Paper", "ProductName", Room.Kitchen, Category.Other, 5, "user1");
            var otherShortage = new Shortage("Soap", "ProductName2", Room.Bathroom, Category.Other, 3, "user2");
            _shortages.Add(userShortage);
            _shortages.Add(otherShortage);

            // Act
            var result = _shortageService.FindShortages("user1", false);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(userShortage, result[0]);
        }

        [TestMethod]
        public void FindShortages_Admin_ReturnsAllShortages()
        {
            // Arrange
            var shortage1 = new Shortage("Paper", "ProductName", Room.Kitchen, Category.Other, 5, "user1");
            var shortage2 = new Shortage("Soap", "ProductName2", Room.Bathroom, Category.Other, 3, "user2");
            _shortages.Add(shortage1);
            _shortages.Add(shortage2);

            // Act
            var result = _shortageService.FindShortages("user1", true);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(shortage1));
            Assert.IsTrue(result.Contains(shortage2));
        }

        [TestMethod]
        public void FindShortages_WithTitleFilter_ReturnsFilteredShortages()
        {
            // Arrange
            var shortage1 = new Shortage("ProductName Paper ", "Paper", Room.Kitchen, Category.Other, 5, "user1");
            var shortage2 = new Shortage("Soap", "ProductName2", Room.Bathroom, Category.Other, 3, "user1");
            _shortages.Add(shortage1);
            _shortages.Add(shortage2);

            // Act
            var result = _shortageService.FindShortages("user1", false, titleFilter: "Paper");

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(shortage1, result[0]);
        }

        [TestMethod]
        public void FindShortages_WithDateRangeFilter_ReturnsFilteredShortages()
        {
            // Arrange
            var shortage1 = new Shortage("Paper", "ProductName", Room.Kitchen, Category.Other, 5, "user1") { CreatedOn = new DateTime(2025, 8, 20) };
            var shortage2 = new Shortage("Soap", "ProductName2", Room.Bathroom, Category.Other, 3, "user1") { CreatedOn = new DateTime(2025, 8, 22) };
            _shortages.Add(shortage1);
            _shortages.Add(shortage2);

            // Act
            var result = _shortageService.FindShortages("user1", false, fromDate: new DateTime(2025, 8, 21));

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(shortage2, result[0]);
        }

        [TestMethod]
        public void FindShortages_WithCategoryAndRoomFilter_ReturnsFilteredShortages()
        {
            // Arrange
            var shortage1 = new Shortage("Paper", "ProductName", Room.Kitchen, Category.Other, 5, "user1");
            var shortage2 = new Shortage("Soap", "ProductName2", Room.Bathroom, Category.Other, 3, "user1");
            _shortages.Add(shortage1);
            _shortages.Add(shortage2);

            // Act
            var result = _shortageService.FindShortages("user1", false, categoryFilter: Category.Other, roomFilter: Room.Kitchen);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(shortage1, result[0]);
        }

        [TestMethod]
        public void FindShortages_SortedByPriorityDescending()
        {
            // Arrange
            var shortage1 = new Shortage("Paper", "ProductName", Room.Kitchen, Category.Other, 5, "user1");
            var shortage2 = new Shortage("Soap", "ProductName2", Room.Bathroom, Category.Other, 3, "user1");
            _shortages.Add(shortage1);
            _shortages.Add(shortage2);

            // Act
            var result = _shortageService.FindShortages("user1", false);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(shortage1, result[0]);
            Assert.AreEqual(shortage2, result[1]);
        }
    }
}