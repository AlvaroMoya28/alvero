using Xunit;
using EventosBackend.Services;
using System;
using System.Linq;

namespace EventosBackend.Tests.Services
{
    public class PasswordHasherExtendedTests
    {
        private readonly PasswordHasher _hasher;

        public PasswordHasherExtendedTests()
        {
            _hasher = new PasswordHasher();
        }

        [Fact]
        public void CreateHash_WithShortPassword_CreatesHash()
        {
            // Arrange
            var password = "Pass1!";

            // Act
            var (hash, salt) = _hasher.CreateHash(password);

            // Assert
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
            Assert.NotNull(salt);
            Assert.NotEmpty(salt);
        }

        [Fact]
        public void CreateHash_WithLongPassword_CreatesHash()
        {
            // Arrange
            var password = new string('A', 100) + "1!";

            // Act
            var (hash, salt) = _hasher.CreateHash(password);

            // Assert
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
            Assert.NotNull(salt);
            Assert.NotEmpty(salt);
        }

        [Fact]
        public void CreateHash_WithSpecialCharacters_CreatesHash()
        {
            // Arrange
            var password = "P@$$w0rd!#%&*()[]{}";

            // Act
            var (hash, salt) = _hasher.CreateHash(password);

            // Assert
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
        }

        [Fact]
        public void CreateHash_WithUnicodeCharacters_CreatesHash()
        {
            // Arrange
            var password = "Pásswörd123!";

            // Act
            var (hash, salt) = _hasher.CreateHash(password);

            // Assert
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "TestPassword123!";
            var (hash, salt) = _hasher.CreateHash(password);

            // Act
            var result = _hasher.VerifyPassword(password, hash, salt);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var correctPassword = "CorrectPassword123!";
            var incorrectPassword = "WrongPassword123!";
            var (hash, salt) = _hasher.CreateHash(correctPassword);

            // Act
            var result = _hasher.VerifyPassword(incorrectPassword, hash, salt);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_WithCaseSensitive_ReturnsFalse()
        {
            // Arrange
            var password = "Password123!";
            var wrongCasePassword = "password123!";
            var (hash, salt) = _hasher.CreateHash(password);

            // Act
            var result = _hasher.VerifyPassword(wrongCasePassword, hash, salt);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_WithEmptyPassword_ReturnsFalse()
        {
            // Arrange
            var password = "Password123!";
            var (hash, salt) = _hasher.CreateHash(password);

            // Act
            var result = _hasher.VerifyPassword(string.Empty, hash, salt);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CreateHash_SaltIsRandomAndUnique()
        {
            // Arrange
            var password = "SamePassword123!";

            // Act
            var (hash1, salt1) = _hasher.CreateHash(password);
            var (hash2, salt2) = _hasher.CreateHash(password);
            var (hash3, salt3) = _hasher.CreateHash(password);

            // Assert
            Assert.NotEqual(salt1, salt2);
            Assert.NotEqual(salt2, salt3);
            Assert.NotEqual(salt1, salt3);
            Assert.NotEqual(hash1, hash2);
            Assert.NotEqual(hash2, hash3);
        }

        [Fact]
        public void VerifyPassword_WithWrongSalt_ReturnsFalse()
        {
            // Arrange
            var password = "Password123!";
            var (hash, correctSalt) = _hasher.CreateHash(password);
            var (_, wrongSalt) = _hasher.CreateHash("DifferentPassword");

            // Act
            var result = _hasher.VerifyPassword(password, hash, wrongSalt);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CreateHash_GeneratesConsistentHashLength()
        {
            // Arrange
            var password1 = "Short1!";
            var password2 = new string('A', 100) + "1!";

            // Act
            var (hash1, _) = _hasher.CreateHash(password1);
            var (hash2, _) = _hasher.CreateHash(password2);

            // Assert
            Assert.Equal(hash1.Length, hash2.Length);
        }

        [Fact]
        public void CreateHash_GeneratesConsistentSaltLength()
        {
            // Arrange
            var password1 = "Password1!";
            var password2 = "DifferentPassword2!";
            var password3 = "AnotherOne3!";

            // Act
            var (_, salt1) = _hasher.CreateHash(password1);
            var (_, salt2) = _hasher.CreateHash(password2);
            var (_, salt3) = _hasher.CreateHash(password3);

            // Assert
            Assert.Equal(salt1.Length, salt2.Length);
            Assert.Equal(salt2.Length, salt3.Length);
        }

        [Theory]
        [InlineData("Password1!")]
        [InlineData("AnotherPass2@")]
        [InlineData("ComplexP@$$w0rd")]
        [InlineData("Simple123")]
        public void VerifyPassword_MultiplePasswords_WorksCorrectly(string password)
        {
            // Arrange
            var (hash, salt) = _hasher.CreateHash(password);

            // Act
            var correctResult = _hasher.VerifyPassword(password, hash, salt);
            var incorrectResult = _hasher.VerifyPassword(password + "wrong", hash, salt);

            // Assert
            Assert.True(correctResult);
            Assert.False(incorrectResult);
        }
    }
}
