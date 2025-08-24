using Moq;
using Praktika2025Vasara.Entities;
using Praktika2025Vasara.Services;
using Praktika2025Vasara.Store;

namespace Praktika2025Vasara.Tests
{
    [TestClass]
    public class UserServiceTests
    {
        private Mock<IFileStore<User>> _mockStore;
        private UserService _userService;
        private List<User> _users;

        [TestInitialize]
        public void Setup()
        {
            _mockStore = new Mock<IFileStore<User>>();
            _users = new List<User>();
            _mockStore.Setup(m => m.GetItems()).Returns(_users);
            _mockStore.Setup(m => m.SaveItems(It.IsAny<List<User>>())).Callback<List<User>>(items => _users = items);
            _userService = new UserService(_mockStore.Object);
        }

        [TestMethod]
        public void IsUserValid_ValidCredentials_ReturnsTrue()
        {
            // Arrange
            var user = new User("testuser", "password123") { Role = Role.User };
            _users.Add(user);
            var inputUser = new User("testuser", "password123");

            // Act
            var result = _userService.IsUserValid(inputUser);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsUserValid_InvalidUsername_ReturnsFalse()
        {
            // Arrange
            var user = new User("testuser", "password123") { Role = Role.User };
            _users.Add(user);
            var inputUser = new User("wronguser", "password123");

            // Act
            var result = _userService.IsUserValid(inputUser);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsUserValid_InvalidPassword_ReturnsFalse()
        {
            // Arrange
            var user = new User("testuser", "password123") { Role = Role.User };
            _users.Add(user);
            var inputUser = new User("testuser", "wrongpassword");

            // Act
            var result = _userService.IsUserValid(inputUser);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetUserByCredentials_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var user = new User("testuser", "password123") { Role = Role.User };
            _users.Add(user);

            // Act
            var result = _userService.GetUserByCredentials("testuser", "password123");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.UserName, result.UserName);
            Assert.AreEqual(user.Password, result.Password);
            Assert.AreEqual(user.Role, result.Role);
        }

        [TestMethod]
        public void GetUserByCredentials_CaseInsensitiveUsername_ReturnsUser()
        {
            // Arrange
            var user = new User("testuser", "password123") { Role = Role.User };
            _users.Add(user);

            // Act
            var result = _userService.GetUserByCredentials("TESTUSER", "password123");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.UserName, result.UserName);
        }

        [TestMethod]
        public void GetUserByCredentials_InvalidCredentials_ReturnsNull()
        {
            // Arrange
            var user = new User("testuser", "password123") { Role = Role.User };
            _users.Add(user);

            // Act
            var result = _userService.GetUserByCredentials("testuser", "wrongpassword");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void AddNewUser_NewUser_AddsToStore()
        {
            // Arrange
            var newUser = new User("testuser", "password123") { Role = Role.User };

            // Act
            _userService.AddNewUser(newUser);

            // Assert
            Assert.AreEqual(1, _users.Count);
            Assert.AreEqual(newUser, _users[0]);
            _mockStore.Verify(m => m.SaveItems(It.IsAny<List<User>>()), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddNewUser_DuplicateUsername_ThrowsException()
        {
            // Arrange
            var existingUser = new User("testuser", "password123") { Role = Role.User };
            _users.Add(existingUser);
            var newUser = new User("testuser", "newpassword") { Role = Role.User };

            // Act
            _userService.AddNewUser(newUser);
        }
    }
}