using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestingProject.UserControlers;
using TestingProject;

namespace TestingProject.UserControlers
{
    [TestFixture]
    public class User_ForcePasswordChange : UserController
    {
        private PasswordService passwordService;

        [SetUp]
        public void SetupUserForcePasswordChange()
        {
            passwordService = new PasswordService();
        }

        [Test]
        [Category("Regression")]
        [Category("Acceptance")]
        public void ForcePasswordChange_ValidNewPassword_ShouldSucceed()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var newPassword = "NewSecurePassword123!";
            passwordService.CreateTestUser(userId, "oldpassword");

            // Act
            var result = passwordService.ForcePasswordChange(userId, newPassword);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsTrue(passwordService.VerifyPassword(userId, newPassword));
        }

        [Test]
        [Category("Regression")]
        [Category("Acceptance")]
        public void ForcePasswordChange_WeakPassword_ShouldFail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var weakPassword = "123";

            // Act
            var result = passwordService.ForcePasswordChange(userId, weakPassword);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Errors.Contains("Password does not meet security requirements"));
        }

        [Test]
        [Category("Regression")]
        [Category("Acceptance")]
        public void ForcePasswordChange_SameAsCurrentPassword_ShouldFail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var currentPassword = "CurrentPassword123!";
            passwordService.CreateTestUser(userId, currentPassword);

            // Act
            var result = passwordService.ForcePasswordChange(userId, currentPassword);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Errors.Contains("New password cannot be the same as current password"));
        }
    }
}