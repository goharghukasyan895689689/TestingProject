global using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestingProject;

namespace TestingProject.UserControlers
{
    [TestFixture]
    public class UserController
    {
        [SetUp]
        public void Setup()
        {
            // Common setup for all User controller tests
            Console.WriteLine("Setting up User Controller tests");
        }

        [TearDown]
        public void TearDown()
        {
            // Common cleanup for all User controller tests
            Console.WriteLine("Tearing down User Controller tests");
        }
    }

    // User_Create Class - Handles user creation actions
    [TestFixture]
    public class User_Create : UserController
    {
        private UserService userService;

        [SetUp]
        public void SetupUserCreate()
        {
            userService = new UserService();
        }

        [Test]
        [Category("Smoke")]
        [Category("Acceptance")]
        [TestCase("john.doe@email.com", "John", "Doe")]
        [TestCase("jane.smith@email.com", "Jane", "Smith")]
        public void CreateUser_ValidData_ShouldSucceed(string email, string firstName, string lastName)
        {
            // Arrange
            var userData = new CreateUserRequest
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Password = "SecurePassword123!"
            };

            // Act
            var result = userService.CreateUser(userData);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.UserId);
            Assert.AreEqual(email, result.User.Email);
        }

        [Test]
        [Category("Smoke")]
        [Category("Acceptance")]
        public void CreateUser_InvalidEmail_ShouldFail()
        {
            // Arrange
            var userData = new CreateUserRequest
            {
                Email = "invalid-email",
                FirstName = "John",
                LastName = "Doe",
                Password = "SecurePassword123!"
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => userService.CreateUser(userData));
        }

        [Test]
        [Category("Smoke")]
        [Category("Acceptance")]
        public void CreateUser_WeakPassword_ShouldFail()
        {
            // Arrange
            var userData = new CreateUserRequest
            {
                Email = "john.doe@email.com",
                FirstName = "John",
                LastName = "Doe",
                Password = "123"
            };

            // Act
            var result = userService.CreateUser(userData);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Errors.Contains("Password does not meet security requirements"));
        }
    }
}