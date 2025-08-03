using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestingProject.UserControlers;
using TestingProject;

namespace TestingProject.UserControlers
{
    [TestFixture]
    public class User_GeneralInfo : UserController
    {
        private UserInfoService userInfoService;

        [SetUp]
        public void SetupUserGeneralInfo()
        {
            userInfoService = new UserInfoService();
        }

        [Test]
        [Category("Smoke")]
        [Category("Acceptance")]
        public void GetUserInfo_ValidUserId_ShouldReturnUserData()
        {
            // Arrange
            var userId = Guid.NewGuid();
            userInfoService.CreateTestUser(userId, "test@email.com", "Test", "User");

            // Act
            var userInfo = userInfoService.GetUserInfo(userId);

            // Assert
            Assert.IsNotNull(userInfo);
            Assert.AreEqual("test@email.com", userInfo.Email);
            Assert.AreEqual("Test", userInfo.FirstName);
            Assert.AreEqual("User", userInfo.LastName);
        }

        [Test]
        [Category("Smoke")]
        [Category("Acceptance")]
        public void UpdateUserInfo_ValidData_ShouldUpdateSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            userInfoService.CreateTestUser(userId, "old@email.com", "Old", "Name");

            var updateRequest = new UpdateUserInfoRequest
            {
                UserId = userId,
                FirstName = "New",
                LastName = "Name",
                PhoneNumber = "+1234567890"
            };

            // Act
            var result = userInfoService.UpdateUserInfo(updateRequest);

            // Assert
            Assert.IsTrue(result.Success);
            var updatedUser = userInfoService.GetUserInfo(userId);
            Assert.AreEqual("New", updatedUser.FirstName);
            Assert.AreEqual("Name", updatedUser.LastName);
            Assert.AreEqual("+1234567890", updatedUser.PhoneNumber);
        }

        [Test]
        [Category("Smoke")]
        [Category("Acceptance")]
        public void GetUserInfo_NonExistentUser_ShouldReturnNull()
        {
            // Arrange
            var nonExistentUserId = Guid.NewGuid();

            // Act
            var userInfo = userInfoService.GetUserInfo(nonExistentUserId);

            // Assert
            Assert.IsNull(userInfo);
        }
    }
}