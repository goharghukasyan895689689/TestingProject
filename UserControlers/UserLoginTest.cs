using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestingProject.UserControlers;
using TestingProject;

namespace TestingProject.UserControlers
{
    [TestFixture]
    public class User_Login : UserController
    {
        private AuthenticationService authService;

        [SetUp]
        public void SetupUserLogin()
        {
            authService = new AuthenticationService();
        }

        [Test]
        [Category("Acceptance")]
        [Category("Regression")]
        [TestCase("user@test.com", "correctpassword", true)]
        [TestCase("user@test.com", "wrongpassword", false)]
        [TestCase("nonexistent@test.com", "anypassword", false)]
        public void Login_VariousCredentials_ShouldReturnExpectedResult(string email, string password, bool expectedSuccess)
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            // Act
            var result = authService.Login(loginRequest);

            // Assert
            Assert.AreEqual(expectedSuccess, result.Success);
            if (expectedSuccess)
            {
                Assert.IsNotNull(result.Token);
                Assert.IsNotNull(result.User);
            }
        }

        [Test]
        [Category("Acceptance")]
        [Category("Regression")]
        public async Task Login_AccountLocked_ShouldReturnLockoutError()
        {
            // Arrange
            var lockedUserEmail = "locked@test.com";
            await authService.LockUserAccount(lockedUserEmail);

            var loginRequest = new LoginRequest
            {
                Email = lockedUserEmail,
                Password = "correctpassword"
            };

            // Act
            var result = authService.Login(loginRequest);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Account is locked", result.ErrorMessage);
        }

        [Test]
        [Category("Acceptance")]
        [Category("Regression")]
        public void Login_MultipleFailedAttempts_ShouldLockAccount()
        {
            // Arrange
            var email = "test@example.com";
            var wrongPassword = "wrongpassword";

            // Act - Attempt login 5 times with wrong password
            for (int i = 0; i < 5; i++)
            {
                authService.Login(new LoginRequest { Email = email, Password = wrongPassword });
            }

            // Final attempt should show account locked
            var finalResult = authService.Login(new LoginRequest { Email = email, Password = "correctpassword" });

            // Assert
            Assert.IsFalse(finalResult.Success);
            Assert.AreEqual("Account is locked", finalResult.ErrorMessage);
        }
    }
}