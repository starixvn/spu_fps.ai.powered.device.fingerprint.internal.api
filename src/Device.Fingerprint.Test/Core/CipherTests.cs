using Device.Fingerprint.Core;
using FluentAssertions;
using System;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Device.Fingerprint.Test.Core
{
    public class CipherTests
    {
        #region Encrypt Tests

        [Fact]
        public void Encrypt_WithValidPlainText_ShouldReturnBase64EncryptedString()
        {
            // Arrange
            string plainText = "Hello World";

            // Act
            var result = Cipher.Encrypt(plainText);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            // Base64 strings should only contain valid Base64 characters
            result.Should().MatchRegex("^[A-Za-z0-9+/=]+$");
        }

        [Fact]
        public void Encrypt_WithNullInput_ShouldReturnNull()
        {
            // Arrange
            string plainText = null!;

            // Act
            var result = Cipher.Encrypt(plainText);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Encrypt_WithEmptyString_ShouldReturnNull()
        {
            // Arrange
            string plainText = string.Empty;

            // Act
            var result = Cipher.Encrypt(plainText);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Encrypt_WithWhitespaceString_ShouldReturnNull()
        {
            // Arrange
            string plainText = "   ";

            // Act
            var result = Cipher.Encrypt(plainText);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Encrypt_WithSpecialCharacters_ShouldEncryptSuccessfully()
        {
            // Arrange
            string plainText = "Special chars: !@#$%^&*()_+-=[]{}|;':\",./<>?";

            // Act
            var result = Cipher.Encrypt(plainText);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().MatchRegex("^[A-Za-z0-9+/=]+$");
        }

        [Fact]
        public void Encrypt_WithUnicodeCharacters_ShouldEncryptSuccessfully()
        {
            // Arrange
            string plainText = "Hello 世界 🌍";

            // Act
            var result = Cipher.Encrypt(plainText);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().MatchRegex("^[A-Za-z0-9+/=]+$");
        }

        [Fact]
        public void Encrypt_WithLongString_ShouldEncryptSuccessfully()
        {
            // Arrange
            string plainText = new string('A', 1000);

            // Act
            var result = Cipher.Encrypt(plainText);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().MatchRegex("^[A-Za-z0-9+/=]+$");
        }

        [Fact]
        public void Encrypt_WithSameInput_ShouldProduceSameOutput()
        {
            // Arrange
            string plainText = "Test String";

            // Act
            var result1 = Cipher.Encrypt(plainText);
            var result2 = Cipher.Encrypt(plainText);

            // Assert
            // The encryption uses a fixed salt and deterministic IV derivation, so same input produces same output
            result1.Should().Be(result2);
        }

        #endregion

        #region Decrypt Tests

        [Fact]
        public void Decrypt_WithValidEncryptedText_ShouldReturnOriginalPlainText()
        {
            // Arrange
            string plainText = "Hello World";
            string encryptedText = Cipher.Encrypt(plainText);

            // Act
            var result = Cipher.Decrypt(encryptedText);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decrypt_WithNullInput_ShouldReturnNull()
        {
            // Arrange
            string encryptedText = null!;

            // Act
            var result = Cipher.Decrypt(encryptedText);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Decrypt_WithEmptyString_ShouldReturnNull()
        {
            // Arrange
            string encryptedText = string.Empty;

            // Act
            var result = Cipher.Decrypt(encryptedText);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Decrypt_WithWhitespaceString_ShouldReturnNull()
        {
            // Arrange
            string encryptedText = "   ";

            // Act
            var result = Cipher.Decrypt(encryptedText);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Decrypt_WithSpecialCharacters_ShouldDecryptCorrectly()
        {
            // Arrange
            string plainText = "Special chars: !@#$%^&*()_+-=[]{}|;':\",./<>?";
            string encryptedText = Cipher.Encrypt(plainText);

            // Act
            var result = Cipher.Decrypt(encryptedText);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decrypt_WithUnicodeCharacters_ShouldDecryptCorrectly()
        {
            // Arrange
            string plainText = "Hello 世界 🌍";
            string encryptedText = Cipher.Encrypt(plainText);

            // Act
            var result = Cipher.Decrypt(encryptedText);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decrypt_WithLongString_ShouldDecryptCorrectly()
        {
            // Arrange
            string plainText = new string('A', 1000);
            string encryptedText = Cipher.Encrypt(plainText);

            // Act
            var result = Cipher.Decrypt(encryptedText);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decrypt_WithInvalidBase64String_ShouldThrowFormatException()
        {
            // Arrange
            string invalidEncryptedText = "This is not valid base64!!!";

            // Act & Assert
            Action act = () => Cipher.Decrypt(invalidEncryptedText);
            act.Should().Throw<FormatException>();
        }

        [Fact]
        public void Decrypt_WithCorruptedEncryptedData_ShouldThrowException()
        {
            // Arrange
            string plainText = "Test";
            string encryptedText = Cipher.Encrypt(plainText);
            // Corrupt the encrypted data by removing last character
            string corruptedEncryptedText = encryptedText.Substring(0, encryptedText.Length - 1);

            // Act & Assert
            Action act = () => Cipher.Decrypt(corruptedEncryptedText);
            act.Should().Throw<Exception>();
        }

        #endregion

        #region Encrypt/Decrypt Round-Trip Tests

        [Fact]
        public void EncryptDecrypt_RoundTrip_ShouldReturnOriginalText()
        {
            // Arrange
            string plainText = "Round trip test";

            // Act
            string encrypted = Cipher.Encrypt(plainText);
            string decrypted = Cipher.Decrypt(encrypted);

            // Assert
            decrypted.Should().Be(plainText);
        }

        [Fact]
        public void EncryptDecrypt_RoundTrip_WithEmptyString_ShouldHandleGracefully()
        {
            // Arrange
            string plainText = "";

            // Act
            string? encrypted = Cipher.Encrypt(plainText);
            string? decrypted = encrypted != null ? Cipher.Decrypt(encrypted) : null;

            // Assert
            encrypted.Should().BeNull();
            decrypted.Should().BeNull();
        }

        [Fact]
        public void EncryptDecrypt_RoundTrip_WithSingleCharacter_ShouldWork()
        {
            // Arrange
            string plainText = "A";

            // Act
            string encrypted = Cipher.Encrypt(plainText);
            string decrypted = Cipher.Decrypt(encrypted);

            // Assert
            decrypted.Should().Be(plainText);
        }

        [Fact]
        public void EncryptDecrypt_RoundTrip_WithNewlines_ShouldWork()
        {
            // Arrange
            string plainText = "Line 1\nLine 2\r\nLine 3";

            // Act
            string encrypted = Cipher.Encrypt(plainText);
            string decrypted = Cipher.Decrypt(encrypted);

            // Assert
            decrypted.Should().Be(plainText);
        }

        [Fact]
        public void EncryptDecrypt_RoundTrip_WithJsonString_ShouldWork()
        {
            // Arrange
            string plainText = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}";

            // Act
            string encrypted = Cipher.Encrypt(plainText);
            string decrypted = Cipher.Decrypt(encrypted);

            // Assert
            decrypted.Should().Be(plainText);
        }

        #endregion

        #region Hashing Tests

        [Fact]
        public void Hashing_WithValidString_ShouldReturnSha256Hash()
        {
            // Arrange
            string original = "Hello World";

            // Act
            var result = Cipher.Hashing(original);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            // SHA256 produces 64 hex characters
            result.Should().HaveLength(64);
            result.Should().MatchRegex("^[0-9a-f]{64}$");
        }

        [Fact]
        public void Hashing_WithSameInput_ShouldProduceSameHash()
        {
            // Arrange
            string original = "Test String";

            // Act
            var result1 = Cipher.Hashing(original);
            var result2 = Cipher.Hashing(original);

            // Assert
            result1.Should().Be(result2);
        }

        [Fact]
        public void Hashing_WithDifferentInputs_ShouldProduceDifferentHashes()
        {
            // Arrange
            string original1 = "Test String 1";
            string original2 = "Test String 2";

            // Act
            var result1 = Cipher.Hashing(original1);
            var result2 = Cipher.Hashing(original2);

            // Assert
            result1.Should().NotBe(result2);
        }

        [Fact]
        public void Hashing_WithEmptyString_ShouldReturnHash()
        {
            // Arrange
            string original = string.Empty;

            // Act
            var result = Cipher.Hashing(original);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveLength(64);
            result.Should().MatchRegex("^[0-9a-f]{64}$");
        }

        [Fact]
        public void Hashing_WithSpecialCharacters_ShouldReturnHash()
        {
            // Arrange
            string original = "!@#$%^&*()_+-=[]{}|;':\",./<>?";

            // Act
            var result = Cipher.Hashing(original);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveLength(64);
            result.Should().MatchRegex("^[0-9a-f]{64}$");
        }

        [Fact]
        public void Hashing_WithUnicodeCharacters_ShouldReturnHash()
        {
            // Arrange
            string original = "Hello 世界 🌍";

            // Act
            var result = Cipher.Hashing(original);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveLength(64);
            result.Should().MatchRegex("^[0-9a-f]{64}$");
        }

        [Fact]
        public void Hashing_WithLongString_ShouldReturnHash()
        {
            // Arrange
            string original = new string('A', 10000);

            // Act
            var result = Cipher.Hashing(original);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveLength(64);
            result.Should().MatchRegex("^[0-9a-f]{64}$");
        }

        [Fact]
        public void Hashing_WithNullInput_ShouldThrowException()
        {
            // Arrange
            string original = null!;

            // Act & Assert
            Action act = () => Cipher.Hashing(original);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Hashing_ShouldMatchExpectedSha256Hash()
        {
            // Arrange
            string original = "Hello World";
            // Expected SHA256 hash of "Hello World"
            string expectedHash = "a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e";

            // Act
            var result = Cipher.Hashing(original);

            // Assert
            result.Should().Be(expectedHash);
        }

        #endregion

        #region Hashing2Int Tests

        [Fact]
        public void Hashing2Int_WithValidString_ShouldReturnInt64()
        {
            // Arrange
            string original = "Hello World";

            // Act
            var result = Cipher.Hashing2Int(original);

            // Assert
            result.Should().BeOfType(typeof(Int64));
        }

        [Fact]
        public void Hashing2Int_WithSameInput_ShouldProduceSameInt()
        {
            // Arrange
            string original = "Test String";

            // Act
            var result1 = Cipher.Hashing2Int(original);
            var result2 = Cipher.Hashing2Int(original);

            // Assert
            result1.Should().Be(result2);
        }

        [Fact]
        public void Hashing2Int_WithDifferentInputs_ShouldProduceDifferentInts()
        {
            // Arrange
            string original1 = "Test String 1";
            string original2 = "Test String 2";

            // Act
            var result1 = Cipher.Hashing2Int(original1);
            var result2 = Cipher.Hashing2Int(original2);

            // Assert
            result1.Should().NotBe(result2);
        }

        [Fact]
        public void Hashing2Int_WithEmptyString_ShouldReturnInt()
        {
            // Arrange
            string original = string.Empty;

            // Act
            var result = Cipher.Hashing2Int(original);

            // Assert
            result.Should().BeOfType(typeof(Int64));
        }

        [Fact]
        public void Hashing2Int_WithSpecialCharacters_ShouldReturnInt()
        {
            // Arrange
            string original = "!@#$%^&*()_+-=[]{}|;':\",./<>?";

            // Act
            var result = Cipher.Hashing2Int(original);

            // Assert
            result.Should().BeOfType(typeof(Int64));
        }

        [Fact]
        public void Hashing2Int_WithUnicodeCharacters_ShouldReturnInt()
        {
            // Arrange
            string original = "Hello 世界 🌍";

            // Act
            var result = Cipher.Hashing2Int(original);

            // Assert
            result.Should().BeOfType(typeof(Int64));
        }

        [Fact]
        public void Hashing2Int_WithLongString_ShouldReturnInt()
        {
            // Arrange
            string original = new string('A', 10000);

            // Act
            var result = Cipher.Hashing2Int(original);

            // Assert
            result.Should().BeOfType(typeof(Int64));
        }

        [Fact]
        public void Hashing2Int_WithNullInput_ShouldThrowException()
        {
            // Arrange
            string original = null!;

            // Act & Assert
            Action act = () => Cipher.Hashing2Int(original);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Hashing2Int_ShouldConvertFirst8BytesOfHashToInt64()
        {
            // Arrange
            string original = "Hello World";
            // Get the SHA256 hash bytes
            byte[] hashBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(original));
            Int64 expectedInt = BitConverter.ToInt64(hashBytes, 0);

            // Act
            var result = Cipher.Hashing2Int(original);

            // Assert
            result.Should().Be(expectedInt);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void EncryptDecryptHashing_Integration_ShouldWorkTogether()
        {
            // Arrange
            string plainText = "Integration test";
            string hash = Cipher.Hashing(plainText);
            Int64 hashInt = Cipher.Hashing2Int(plainText);

            // Act
            string encrypted = Cipher.Encrypt(plainText);
            string decrypted = Cipher.Decrypt(encrypted);

            // Assert
            decrypted.Should().Be(plainText);
            hash.Should().NotBeNullOrEmpty();
            hashInt.Should().NotBe(0);
        }

        [Fact]
        public void HashingAndHashing2Int_ShouldBeConsistent()
        {
            // Arrange
            string original = "Consistency test";
            string hash = Cipher.Hashing(original);
            Int64 hashInt = Cipher.Hashing2Int(original);

            // Act - Get hash bytes and convert first 8 bytes to Int64
            byte[] hashBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(original));
            Int64 expectedInt = BitConverter.ToInt64(hashBytes, 0);

            // Assert
            hash.Should().NotBeNullOrEmpty();
            hashInt.Should().Be(expectedInt);
        }

        #endregion
    }
}

