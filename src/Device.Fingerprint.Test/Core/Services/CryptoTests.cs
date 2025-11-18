using Device.Fingerprint.Core;
using Device.Fingerprint.Core.Services;
using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace Device.Fingerprint.Test.Core.Services
{
    public class CryptoTests : IDisposable
    {
        private readonly Mock<IAppSettings> _mockAppSettings;
        private Crypto _crypto;
        private bool _disposed;

        public CryptoTests()
        {
            _mockAppSettings = new Mock<IAppSettings>();
            _mockAppSettings.Setup(x => x.CryptoIV).Returns("c3eef06037e837f4ee390bda69c05023");
            _mockAppSettings.Setup(x => x.CryptoSalt).Returns("234292a9fce031f2");
            _mockAppSettings.Setup(x => x.CryptoPassword).Returns("TestPassword123");
            
            _crypto = new Crypto(_mockAppSettings.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ShouldInitializeCrypto_WithValidSettings()
        {
            // Act
            var crypto = new Crypto(_mockAppSettings.Object);

            // Assert
            crypto.Should().NotBeNull();
            crypto.Should().BeAssignableTo<ICrypto>();
        }

        [Fact]
        public void Constructor_ShouldUseDefaultIV_WhenIVIsNull()
        {
            // Arrange
            var mockSettings = new Mock<IAppSettings>();
            mockSettings.Setup(x => x.CryptoIV).Returns((string)null);
            mockSettings.Setup(x => x.CryptoSalt).Returns("234292a9fce031f2");
            mockSettings.Setup(x => x.CryptoPassword).Returns("TestPassword123");

            // Act
            var crypto = new Crypto(mockSettings.Object);

            // Assert
            crypto.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_ShouldUseDefaultSalt_WhenSaltIsNull()
        {
            // Arrange
            var mockSettings = new Mock<IAppSettings>();
            mockSettings.Setup(x => x.CryptoIV).Returns("c3eef06037e837f4ee390bda69c05023");
            mockSettings.Setup(x => x.CryptoSalt).Returns((string)null);
            mockSettings.Setup(x => x.CryptoPassword).Returns("TestPassword123");

            // Act
            var crypto = new Crypto(mockSettings.Object);

            // Assert
            crypto.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_ShouldImplementICryptoInterface()
        {
            // Arrange & Act
            var crypto = new Crypto(_mockAppSettings.Object);

            // Assert
            crypto.Should().BeAssignableTo<ICrypto>();
        }

        [Fact]
        public void Constructor_ShouldImplementIDisposableInterface()
        {
            // Arrange & Act
            var crypto = new Crypto(_mockAppSettings.Object);

            // Assert
            crypto.Should().BeAssignableTo<IDisposable>();
        }

        #endregion

        #region Encrypt Tests

        [Fact]
        public void Encrypt_ShouldReturnEncryptedString_WhenGivenValidPlainText()
        {
            // Arrange
            var plainText = "Hello World";

            // Act
            var encrypted = _crypto.Encrypt(plainText);

            // Assert
            encrypted.Should().NotBeNullOrEmpty();
            encrypted.Should().NotBe(plainText);
        }

        [Fact]
        public void Encrypt_ShouldReturnEmptyString_WhenGivenEmptyString()
        {
            // Arrange
            var plainText = string.Empty;

            // Act
            var encrypted = _crypto.Encrypt(plainText);

            // Assert
            encrypted.Should().BeEmpty();
        }

        [Fact]
        public void Encrypt_ShouldReturnEmptyString_WhenGivenNull()
        {
            // Arrange
            string plainText = null;

            // Act
            var encrypted = _crypto.Encrypt(plainText);

            // Assert
            encrypted.Should().BeEmpty();
        }

        [Fact]
        public void Encrypt_ShouldReturnBase64String_WhenCalled()
        {
            // Arrange
            var plainText = "Test String 123";

            // Act
            var encrypted = _crypto.Encrypt(plainText);

            // Assert
            encrypted.Should().NotBeNullOrEmpty();
            Action act = () => Convert.FromBase64String(encrypted);
            act.Should().NotThrow("encrypted string should be valid Base64");
        }

        [Fact]
        public void Encrypt_ShouldHandleSpecialCharacters_WhenCalled()
        {
            // Arrange
            var plainText = "!@#$%^&*()_+-=[]{}|;':\",./<>?";

            // Act
            var encrypted = _crypto.Encrypt(plainText);

            // Assert
            encrypted.Should().NotBeNullOrEmpty();
            encrypted.Should().NotBe(plainText);
        }

        [Fact]
        public void Encrypt_ShouldHandleUnicodeCharacters_WhenCalled()
        {
            // Arrange
            var plainText = "Tiếng Việt 中文 日本語 한국어";

            // Act
            var encrypted = _crypto.Encrypt(plainText);

            // Assert
            encrypted.Should().NotBeNullOrEmpty();
            encrypted.Should().NotBe(plainText);
        }

        [Fact]
        public void Encrypt_ShouldHandleLongText_WhenCalled()
        {
            // Arrange
            var plainText = new string('a', 10000);

            // Act
            var encrypted = _crypto.Encrypt(plainText);

            // Assert
            encrypted.Should().NotBeNullOrEmpty();
            encrypted.Should().NotBe(plainText);
        }

        [Fact]
        public void Encrypt_ShouldProduceDifferentResults_WithDifferentPasswords()
        {
            // Arrange
            var plainText = "Test Data";
            var mockSettings1 = new Mock<IAppSettings>();
            mockSettings1.Setup(x => x.CryptoIV).Returns("c3eef06037e837f4ee390bda69c05023");
            mockSettings1.Setup(x => x.CryptoSalt).Returns("234292a9fce031f2");
            mockSettings1.Setup(x => x.CryptoPassword).Returns("Password1");
            
            var mockSettings2 = new Mock<IAppSettings>();
            mockSettings2.Setup(x => x.CryptoIV).Returns("c3eef06037e837f4ee390bda69c05023");
            mockSettings2.Setup(x => x.CryptoSalt).Returns("234292a9fce031f2");
            mockSettings2.Setup(x => x.CryptoPassword).Returns("Password2");

            using var crypto1 = new Crypto(mockSettings1.Object);
            using var crypto2 = new Crypto(mockSettings2.Object);

            // Act
            var encrypted1 = crypto1.Encrypt(plainText);
            var encrypted2 = crypto2.Encrypt(plainText);

            // Assert
            encrypted1.Should().NotBe(encrypted2, "different passwords should produce different encrypted results");
        }

        #endregion

        #region Decrypt Tests

        [Fact]
        public void Decrypt_ShouldReturnOriginalText_WhenGivenValidEncryptedText()
        {
            // Arrange
            var plainText = "Hello World";
            var encrypted = _crypto.Encrypt(plainText);

            // Act
            var decrypted = _crypto.Decrypt(encrypted);

            // Assert
            decrypted.Should().Be(plainText);
        }

        [Fact]
        public void Decrypt_ShouldReturnEmptyString_WhenGivenEmptyString()
        {
            // Arrange
            var encrypted = string.Empty;

            // Act
            var decrypted = _crypto.Decrypt(encrypted);

            // Assert
            decrypted.Should().BeEmpty();
        }

        [Fact]
        public void Decrypt_ShouldReturnEmptyString_WhenGivenNull()
        {
            // Arrange
            string encrypted = null;

            // Act
            var decrypted = _crypto.Decrypt(encrypted);

            // Assert
            decrypted.Should().BeEmpty();
        }

        [Fact]
        public void Decrypt_ShouldHandleSpecialCharacters_WhenCalled()
        {
            // Arrange
            var plainText = "!@#$%^&*()_+-=[]{}|;':\",./<>?";
            var encrypted = _crypto.Encrypt(plainText);

            // Act
            var decrypted = _crypto.Decrypt(encrypted);

            // Assert
            decrypted.Should().Be(plainText);
        }

        [Fact]
        public void Decrypt_ShouldHandleUnicodeCharacters_WhenCalled()
        {
            // Arrange
            var plainText = "Tiếng Việt 中文 日本語 한국어";
            var encrypted = _crypto.Encrypt(plainText);

            // Act
            var decrypted = _crypto.Decrypt(encrypted);

            // Assert
            decrypted.Should().Be(plainText);
        }

        [Fact]
        public void Decrypt_ShouldHandleLongText_WhenCalled()
        {
            // Arrange
            var plainText = new string('a', 10000);
            var encrypted = _crypto.Encrypt(plainText);

            // Act
            var decrypted = _crypto.Decrypt(encrypted);

            // Assert
            decrypted.Should().Be(plainText);
        }

        [Fact]
        public void Decrypt_ShouldThrowException_WhenGivenInvalidBase64()
        {
            // Arrange
            var invalidEncrypted = "This is not a valid base64 string!";

            // Act
            Action act = () => _crypto.Decrypt(invalidEncrypted);

            // Assert
            act.Should().Throw<FormatException>();
        }

        [Fact]
        public void Decrypt_ShouldThrowException_WhenGivenValidBase64ButInvalidCipherText()
        {
            // Arrange
            var invalidEncrypted = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 });

            // Act
            Action act = () => _crypto.Decrypt(invalidEncrypted);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Decrypt_ShouldFail_WhenUsingDifferentPassword()
        {
            // Arrange
            var plainText = "Secret Message";
            var mockSettings1 = new Mock<IAppSettings>();
            mockSettings1.Setup(x => x.CryptoIV).Returns("c3eef06037e837f4ee390bda69c05023");
            mockSettings1.Setup(x => x.CryptoSalt).Returns("234292a9fce031f2");
            mockSettings1.Setup(x => x.CryptoPassword).Returns("Password1");
            
            var mockSettings2 = new Mock<IAppSettings>();
            mockSettings2.Setup(x => x.CryptoIV).Returns("c3eef06037e837f4ee390bda69c05023");
            mockSettings2.Setup(x => x.CryptoSalt).Returns("234292a9fce031f2");
            mockSettings2.Setup(x => x.CryptoPassword).Returns("Password2");

            using var crypto1 = new Crypto(mockSettings1.Object);
            using var crypto2 = new Crypto(mockSettings2.Object);
            
            var encrypted = crypto1.Encrypt(plainText);

            // Act
            Action act = () => crypto2.Decrypt(encrypted);

            // Assert
            act.Should().Throw<Exception>("decryption should fail with different password");
        }

        #endregion

        #region Encrypt/Decrypt Round Trip Tests

        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("Hello World")]
        [InlineData("1234567890")]
        [InlineData("Special chars: !@#$%^&*()")]
        public void EncryptDecrypt_ShouldReturnOriginalText_ForVariousInputs(string plainText)
        {
            // Act
            var encrypted = _crypto.Encrypt(plainText);
            var decrypted = _crypto.Decrypt(encrypted);

            // Assert
            if (string.IsNullOrEmpty(plainText))
            {
                decrypted.Should().BeEmpty();
            }
            else
            {
                decrypted.Should().Be(plainText);
            }
        }

        [Fact]
        public void EncryptDecrypt_ShouldBeIdempotent_WhenCalledMultipleTimes()
        {
            // Arrange
            var plainText = "Test Data";

            // Act
            var encrypted1 = _crypto.Encrypt(plainText);
            var decrypted1 = _crypto.Decrypt(encrypted1);
            
            var encrypted2 = _crypto.Encrypt(decrypted1);
            var decrypted2 = _crypto.Decrypt(encrypted2);

            // Assert
            decrypted1.Should().Be(plainText);
            decrypted2.Should().Be(plainText);
        }

        #endregion

        #region GetMD5Hash Tests

        [Fact]
        public void GetMD5Hash_ShouldReturnHash_WhenGivenValidString()
        {
            // Arrange
            var input = "Hello World";

            // Act
            var hash = _crypto.GetMD5Hash(input);

            // Assert
            hash.Should().NotBeNullOrEmpty();
            hash.Length.Should().Be(32, "MD5 hash should be 32 hexadecimal characters");
        }

        [Fact]
        public void GetMD5Hash_ShouldReturnEmptyString_WhenGivenEmptyString()
        {
            // Arrange
            var input = string.Empty;

            // Act
            var hash = _crypto.GetMD5Hash(input);

            // Assert
            hash.Should().BeEmpty();
        }

        [Fact]
        public void GetMD5Hash_ShouldReturnEmptyString_WhenGivenNull()
        {
            // Arrange
            string input = null;

            // Act
            var hash = _crypto.GetMD5Hash(input);

            // Assert
            hash.Should().BeEmpty();
        }

        [Fact]
        public void GetMD5Hash_ShouldReturnLowercaseHexString_WhenCalled()
        {
            // Arrange
            var input = "Test String";

            // Act
            var hash = _crypto.GetMD5Hash(input);

            // Assert
            hash.Should().MatchRegex("^[a-f0-9]+$", "hash should contain only lowercase hexadecimal characters");
        }

        [Fact]
        public void GetMD5Hash_ShouldReturnConsistentHash_ForSameInput()
        {
            // Arrange
            var input = "Consistent Test";

            // Act
            var hash1 = _crypto.GetMD5Hash(input);
            var hash2 = _crypto.GetMD5Hash(input);

            // Assert
            hash1.Should().Be(hash2, "same input should always produce same hash");
        }

        [Fact]
        public void GetMD5Hash_ShouldReturnDifferentHashes_ForDifferentInputs()
        {
            // Arrange
            var input1 = "String 1";
            var input2 = "String 2";

            // Act
            var hash1 = _crypto.GetMD5Hash(input1);
            var hash2 = _crypto.GetMD5Hash(input2);

            // Assert
            hash1.Should().NotBe(hash2, "different inputs should produce different hashes");
        }

        [Fact]
        public void GetMD5Hash_ShouldHandleSpecialCharacters_WhenCalled()
        {
            // Arrange
            var input = "!@#$%^&*()_+-=[]{}|;':\",./<>?";

            // Act
            var hash = _crypto.GetMD5Hash(input);

            // Assert
            hash.Should().NotBeNullOrEmpty();
            hash.Length.Should().Be(32);
        }

        [Fact]
        public void GetMD5Hash_ShouldHandleLongStrings_WhenCalled()
        {
            // Arrange
            var input = new string('a', 10000);

            // Act
            var hash = _crypto.GetMD5Hash(input);

            // Assert
            hash.Should().NotBeNullOrEmpty();
            hash.Length.Should().Be(32);
        }

        [Theory]
        [InlineData("test", "098f6bcd4621d373cade4e832627b4f6")]
        [InlineData("password", "5f4dcc3b5aa765d61d8327deb882cf99")]
        [InlineData("hello", "5d41402abc4b2a76b9719d911017c592")]
        public void GetMD5Hash_ShouldReturnKnownHash_ForKnownInputs(string input, string expectedHash)
        {
            // Act
            var hash = _crypto.GetMD5Hash(input);

            // Assert
            hash.Should().Be(expectedHash, "known inputs should produce known MD5 hashes");
        }

        #endregion

        #region HexStringToByteArray Tests

        [Fact]
        public void HexStringToByteArray_ShouldConvertValidHexString_ToByteArray()
        {
            // Arrange
            var hexString = "c3eef06037e837f4ee390bda69c05023";

            // Act
            var bytes = Crypto.HexStringToByteArray(hexString);

            // Assert
            bytes.Should().NotBeNull();
            bytes.Length.Should().Be(16, "32 hex chars should produce 16 bytes");
        }

        [Fact]
        public void HexStringToByteArray_ShouldHandleUppercaseHex_WhenCalled()
        {
            // Arrange
            var hexString = "C3EEF06037E837F4EE390BDA69C05023";

            // Act
            var bytes = Crypto.HexStringToByteArray(hexString);

            // Assert
            bytes.Should().NotBeNull();
            bytes.Length.Should().Be(16);
        }

        [Fact]
        public void HexStringToByteArray_ShouldHandleMixedCaseHex_WhenCalled()
        {
            // Arrange
            var hexString = "C3eEf06037E837f4Ee390BdA69c05023";

            // Act
            var bytes = Crypto.HexStringToByteArray(hexString);

            // Assert
            bytes.Should().NotBeNull();
            bytes.Length.Should().Be(16);
        }

        [Fact]
        public void HexStringToByteArray_ShouldReturnEmptyArray_ForEmptyString()
        {
            // Arrange
            var hexString = string.Empty;

            // Act
            var bytes = Crypto.HexStringToByteArray(hexString);

            // Assert
            bytes.Should().NotBeNull();
            bytes.Length.Should().Be(0);
        }

        [Theory]
        [InlineData("00", new byte[] { 0 })]
        [InlineData("FF", new byte[] { 255 })]
        [InlineData("0F", new byte[] { 15 })]
        [InlineData("F0", new byte[] { 240 })]
        public void HexStringToByteArray_ShouldConvertCorrectly_ForKnownValues(string hexString, byte[] expectedBytes)
        {
            // Act
            var bytes = Crypto.HexStringToByteArray(hexString);

            // Assert
            bytes.Should().BeEquivalentTo(expectedBytes);
        }

        #endregion

        #region Dispose Tests

        [Fact]
        public void Dispose_ShouldNotThrowException_WhenCalled()
        {
            // Arrange
            var crypto = new Crypto(_mockAppSettings.Object);

            // Act
            Action act = () => crypto.Dispose();

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Dispose_ShouldAllowMultipleCalls_WithoutException()
        {
            // Arrange
            var crypto = new Crypto(_mockAppSettings.Object);

            // Act
            Action act = () =>
            {
                crypto.Dispose();
                crypto.Dispose();
                crypto.Dispose();
            };

            // Assert
            act.Should().NotThrow();
        }

        #endregion

        #region Interface Tests

        [Fact]
        public void ICrypto_ShouldHaveEncryptMethod()
        {
            // Arrange
            var method = typeof(ICrypto).GetMethod("Encrypt");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(string));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(string));
        }

        [Fact]
        public void ICrypto_ShouldHaveDecryptMethod()
        {
            // Arrange
            var method = typeof(ICrypto).GetMethod("Decrypt");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(string));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(string));
        }

        [Fact]
        public void ICrypto_ShouldHaveGetMD5HashMethod()
        {
            // Arrange
            var method = typeof(ICrypto).GetMethod("GetMD5Hash");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(string));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(string));
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _crypto?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
