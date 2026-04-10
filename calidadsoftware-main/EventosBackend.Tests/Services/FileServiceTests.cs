using Xunit;
using Moq;
using EventosBackend.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace EventosBackend.Tests.Services
{
    public class FileServiceTests
    {
        private readonly Mock<IWebHostEnvironment> _mockHostingEnvironment;
        private readonly FileService _service;
        private readonly string _testWebRootPath;

        public FileServiceTests()
        {
            _mockHostingEnvironment = new Mock<IWebHostEnvironment>();
            _testWebRootPath = Path.Combine(Path.GetTempPath(), "test_webroot");
            _mockHostingEnvironment.Setup(e => e.WebRootPath).Returns(_testWebRootPath);
            _service = new FileService(_mockHostingEnvironment.Object);

            // Crear directorio temporal para pruebas
            Directory.CreateDirectory(_testWebRootPath);
        }

        [Fact]
        public async Task SaveImageAsync_WithValidImage_SavesFile()
        {
            // Arrange
            var content = "fake image content";
            var fileName = "test.jpg";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(stream.Length);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream s, CancellationToken ct) => stream.CopyToAsync(s));

            // Act
            var result = await _service.SaveImageAsync(mockFile.Object, "test");

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith("/uploads/test/", result);
            Assert.EndsWith(".jpg", result);
        }

        [Fact]
        public async Task SaveImageAsync_WithNullFile_ThrowsException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.SaveImageAsync(null, "test"));
        }

        [Fact]
        public async Task SaveImageAsync_WithEmptyFile_ThrowsException()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(0);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.SaveImageAsync(mockFile.Object, "test"));
        }

        [Fact]
        public async Task SaveImageAsync_WithInvalidExtension_ThrowsException()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.exe");
            mockFile.Setup(f => f.Length).Returns(100);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.SaveImageAsync(mockFile.Object, "test"));
        }

        [Fact]
        public void DeleteFile_WithValidPath_DeletesFile()
        {
            // Arrange
            var testFolder = Path.Combine(_testWebRootPath, "uploads", "test");
            Directory.CreateDirectory(testFolder);
            var testFile = Path.Combine(testFolder, "testfile.jpg");
            File.WriteAllText(testFile, "test content");

            var relativePath = "/uploads/test/testfile.jpg";

            // Act
            _service.DeleteFile(relativePath);

            // Assert
            Assert.False(File.Exists(testFile));
        }

        [Fact]
        public void DeleteFile_WithNullPath_DoesNotThrow()
        {
            // Act & Assert
            var exception = Record.Exception(() => _service.DeleteFile(null));
            Assert.Null(exception);
        }

        [Fact]
        public void DeleteFile_WithEmptyPath_DoesNotThrow()
        {
            // Act & Assert
            var exception = Record.Exception(() => _service.DeleteFile(string.Empty));
            Assert.Null(exception);
        }
    }
}
