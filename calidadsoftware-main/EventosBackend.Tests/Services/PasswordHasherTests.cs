using EventosBackend.Services;
using Xunit;

namespace EventosBackend.Tests.Services
{
    public class PasswordHasherTests
    {
        private readonly PasswordHasher _hasher;

        public PasswordHasherTests()
        {
            _hasher = new PasswordHasher();
        }

        [Fact]
        public void CreateHash_WithValidPassword_ReturnsHashAndSalt()
        {
            // Arrange
            var password = "testpassword123";

            // Act
            var (hash, salt) = _hasher.CreateHash(password);

            // Assert
            Assert.NotNull(hash);
            Assert.NotNull(salt);
            Assert.NotEmpty(hash);
            Assert.NotEmpty(salt);
        }

        [Fact]
        public void CreateHash_WithSamePassword_GeneratesDifferentHashes()
        {
            // Arrange
            var password = "testpassword123";

            // Act
            var (hash1, salt1) = _hasher.CreateHash(password);
            var (hash2, salt2) = _hasher.CreateHash(password);

            // Assert
            Assert.NotEqual(hash1, hash2);
            Assert.NotEqual(salt1, salt2);
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "testpassword123";
            var (hash, salt) = _hasher.CreateHash(password);

            // Act
            var isValid = _hasher.VerifyPassword(password, hash, salt);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var password = "testpassword123";
            var wrongPassword = "wrongpassword";
            var (hash, salt) = _hasher.CreateHash(password);

            // Act
            var isValid = _hasher.VerifyPassword(wrongPassword, hash, salt);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void VerifyPassword_WithEmptyPassword_ReturnsFalse()
        {
            // Arrange
            var password = "testpassword123";
            var (hash, salt) = _hasher.CreateHash(password);

            // Act
            var isValid = _hasher.VerifyPassword("", hash, salt);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void CreateHash_WithLongPassword_Works()
        {
            // Arrange
            var longPassword = new string('a', 1000);

            // Act
            var (hash, salt) = _hasher.CreateHash(longPassword);

            // Assert
            Assert.NotNull(hash);
            Assert.NotNull(salt);
            Assert.True(_hasher.VerifyPassword(longPassword, hash, salt));
        }

        [Fact]
        public void CreateHash_WithSpecialCharacters_Works()
        {
            // Arrange
            var password = "P@ssw0rd!#$%^&*()";

            // Act
            var (hash, salt) = _hasher.CreateHash(password);

            // Assert
            Assert.NotNull(hash);
            Assert.True(_hasher.VerifyPassword(password, hash, salt));
        }

        [Fact]
        public void CreateHash_WithUnicodeCharacters_Works()
        {
            // Arrange
            var password = "contraseña123áéíóú";

            // Act
            var (hash, salt) = _hasher.CreateHash(password);

            // Assert
            Assert.NotNull(hash);
            Assert.True(_hasher.VerifyPassword(password, hash, salt));
        }
    }
}
