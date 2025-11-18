using Device.Fingerprint.Core.Encoders;
using FluentAssertions;

namespace Device.Fingerprint.Test.Core.Encoders
{
    /// <summary>
    /// Unit tests for RC4 encryption class.
    /// Tests encryption, decryption, and edge cases.
    /// </summary>
    public class Rc4Tests
    {
        #region Encrypt Tests

        [Fact]
        public void Encrypt_WithValidKeyAndValue_ShouldReturnEncryptedString()
        {
            // Arrange
            var key = "testkey";
            var value = "Hello World";

            // Act
            var result = Rc4.Encrypt(key, value);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().NotBe(value);
        }

        [Fact]
        public void Encrypt_WithSameKeyAndValue_ShouldReturnSameEncryptedString()
        {
            // Arrange
            var key = "secretkey";
            var value = "Test Message";

            // Act
            var result1 = Rc4.Encrypt(key, value);
            var result2 = Rc4.Encrypt(key, value);

            // Assert
            result1.Should().Be(result2);
        }

        [Fact]
        public void Encrypt_WithDifferentKeys_ShouldReturnDifferentEncryptedStrings()
        {
            // Arrange
            var key1 = "key1";
            var key2 = "key2";
            var value = "Same Value";

            // Act
            var result1 = Rc4.Encrypt(key1, value);
            var result2 = Rc4.Encrypt(key2, value);

            // Assert
            result1.Should().NotBe(result2);
        }

        [Fact]
        public void Encrypt_WithEmptyValue_ShouldReturnEmptyString()
        {
            // Arrange
            var key = "testkey";
            var value = string.Empty;

            // Act
            var result = Rc4.Encrypt(key, value);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Encrypt_WithSingleCharacter_ShouldEncryptSuccessfully()
        {
            // Arrange
            var key = "key";
            var value = "A";

            // Act
            var result = Rc4.Encrypt(key, value);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(1);
            result.Should().NotBe(value);
        }

        [Fact]
        public void Encrypt_WithSpecialCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var key = "specialkey";
            var value = "!@#$%^&*()";

            // Act
            var result = Rc4.Encrypt(key, value);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(value.Length);
        }

        [Fact]
        public void Encrypt_WithNumericString_ShouldEncryptSuccessfully()
        {
            // Arrange
            var key = "numkey";
            var value = "1234567890";

            // Act
            var result = Rc4.Encrypt(key, value);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBe(value);
        }

        [Fact]
        public void Encrypt_WithLongString_ShouldEncryptEntireString()
        {
            // Arrange
            var key = "longkey";
            var value = new string('A', 1000);

            // Act
            var result = Rc4.Encrypt(key, value);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(value.Length);
        }

        [Fact]
        public void Encrypt_WithUnicodeCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var key = "unicode";
            var value = "Hello 世界 🌍";

            // Act
            var result = Rc4.Encrypt(key, value);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(value.Length);
        }

        #endregion

        #region Decrypt Tests

        [Fact]
        public void Decrypt_WithEncryptedValue_ShouldReturnOriginalValue()
        {
            // Arrange
            var key = "decryptkey";
            var originalValue = "Original Message";
            var encrypted = Rc4.Encrypt(key, originalValue);

            // Act
            var decrypted = Rc4.Decrypt(key, encrypted);

            // Assert
            decrypted.Should().Be(originalValue);
        }

        [Fact]
        public void Decrypt_IsSymmetric_EncryptAndDecryptShouldBeIdentical()
        {
            // Arrange
            var key = "symmetrickey";
            var value = "Symmetric Test";

            // Act
            var encrypted = Rc4.Encrypt(key, value);
            var decrypted = Rc4.Decrypt(key, encrypted);

            // Assert
            decrypted.Should().Be(value);
        }

        [Fact]
        public void Decrypt_WithWrongKey_ShouldNotReturnOriginalValue()
        {
            // Arrange
            var correctKey = "correct";
            var wrongKey = "wrong";
            var originalValue = "Secret Data";
            var encrypted = Rc4.Encrypt(correctKey, originalValue);

            // Act
            var decrypted = Rc4.Decrypt(wrongKey, encrypted);

            // Assert
            decrypted.Should().NotBe(originalValue);
        }

        [Fact]
        public void Decrypt_WithEmptyEncryptedValue_ShouldReturnEmptyString()
        {
            // Arrange
            var key = "testkey";
            var encryptedValue = string.Empty;

            // Act
            var result = Rc4.Decrypt(key, encryptedValue);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Decrypt_MultipleRounds_ShouldBeReversible()
        {
            // Arrange
            var key = "roundkey";
            var originalValue = "Round Trip Test";

            // Act
            var encrypted1 = Rc4.Encrypt(key, originalValue);
            var decrypted1 = Rc4.Decrypt(key, encrypted1);
            var encrypted2 = Rc4.Encrypt(key, decrypted1);
            var decrypted2 = Rc4.Decrypt(key, encrypted2);

            // Assert
            decrypted1.Should().Be(originalValue);
            decrypted2.Should().Be(originalValue);
            encrypted1.Should().Be(encrypted2);
        }

        #endregion

        #region Edge Cases and Integration Tests

        [Fact]
        public void Encrypt_WithShortKey_ShouldStillEncrypt()
        {
            // Arrange
            var key = "k";
            var value = "This is a longer message than the key";

            // Act
            var encrypted = Rc4.Encrypt(key, value);
            var decrypted = Rc4.Decrypt(key, encrypted);

            // Assert
            encrypted.Should().NotBe(value);
            decrypted.Should().Be(value);
        }

        [Fact]
        public void Encrypt_WithLongKey_ShouldStillEncrypt()
        {
            // Arrange
            var key = new string('x', 500);
            var value = "Short message";

            // Act
            var encrypted = Rc4.Encrypt(key, value);
            var decrypted = Rc4.Decrypt(key, encrypted);

            // Assert
            encrypted.Should().NotBe(value);
            decrypted.Should().Be(value);
        }

        [Fact]
        public void EncryptDecrypt_WithWhitespace_ShouldPreserveWhitespace()
        {
            // Arrange
            var key = "whitespacekey";
            var value = "  Spaces  and\ttabs\nand\rnewlines  ";

            // Act
            var encrypted = Rc4.Encrypt(key, value);
            var decrypted = Rc4.Decrypt(key, encrypted);

            // Assert
            decrypted.Should().Be(value);
        }

        [Fact]
        public void EncryptDecrypt_WithMixedCaseKey_ShouldBeCaseSensitive()
        {
            // Arrange
            var key1 = "CaseSensitive";
            var key2 = "casesensitive";
            var value = "Test Value";

            // Act
            var encrypted1 = Rc4.Encrypt(key1, value);
            var encrypted2 = Rc4.Encrypt(key2, value);

            // Assert
            encrypted1.Should().NotBe(encrypted2);
        }

        [Fact]
        public void Encrypt_ResultLength_ShouldMatchInputLength()
        {
            // Arrange
            var key = "lengthtest";
            var value = "Check length preservation";

            // Act
            var result = Rc4.Encrypt(key, value);

            // Assert
            result.Length.Should().Be(value.Length);
        }

        [Fact]
        public void EncryptDecrypt_WithJsonString_ShouldPreserveStructure()
        {
            // Arrange
            var key = "jsonkey";
            var value = "{\"name\":\"test\",\"value\":123}";

            // Act
            var encrypted = Rc4.Encrypt(key, value);
            var decrypted = Rc4.Decrypt(key, encrypted);

            // Assert
            decrypted.Should().Be(value);
        }

        [Fact]
        public void EncryptDecrypt_ConsecutiveCalls_ShouldBeConsistent()
        {
            // Arrange
            var key = "consistent";
            var value = "Consistency Test";

            // Act - Perform multiple encrypt/decrypt cycles
            for (int i = 0; i < 10; i++)
            {
                var encrypted = Rc4.Encrypt(key, value);
                var decrypted = Rc4.Decrypt(key, encrypted);

                // Assert
                decrypted.Should().Be(value);
            }
        }

        #endregion

        #region Interface and Method Signature Tests

        [Fact]
        public void Rc4Class_ShouldBeStatic()
        {
            // Arrange & Assert
            typeof(Rc4).Should().BeStatic();
        }

        [Fact]
        public void Rc4_ShouldHaveEncryptMethod()
        {
            // Arrange
            var method = typeof(Rc4).GetMethod("Encrypt");

            // Assert
            method.Should().NotBeNull();
            method!.IsStatic.Should().BeTrue();
            method.ReturnType.Should().Be(typeof(string));
            method.GetParameters().Should().HaveCount(2);
        }

        [Fact]
        public void Rc4_ShouldHaveDecryptMethod()
        {
            // Arrange
            var method = typeof(Rc4).GetMethod("Decrypt");

            // Assert
            method.Should().NotBeNull();
            method!.IsStatic.Should().BeTrue();
            method.ReturnType.Should().Be(typeof(string));
            method.GetParameters().Should().HaveCount(2);
        }

        [Fact]
        public void Rc4_EncryptMethod_ParametersShouldBeStrings()
        {
            // Arrange
            var method = typeof(Rc4).GetMethod("Encrypt");
            var parameters = method!.GetParameters();

            // Assert
            parameters[0].ParameterType.Should().Be(typeof(string));
            parameters[1].ParameterType.Should().Be(typeof(string));
        }

        [Fact]
        public void Rc4_DecryptMethod_ParametersShouldBeStrings()
        {
            // Arrange
            var method = typeof(Rc4).GetMethod("Decrypt");
            var parameters = method!.GetParameters();

            // Assert
            parameters[0].ParameterType.Should().Be(typeof(string));
            parameters[1].ParameterType.Should().Be(typeof(string));
        }

        #endregion
    }
}
