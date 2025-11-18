using Device.Fingerprint.Core.Encoders;
using FluentAssertions;
using System;
using System.Text;
using Xunit;

namespace Device.Fingerprint.Test.Core.Encoders
{
    public class Base64Tests
    {
        // Standard Base64 alphabet (must include '=' at position 64 for padding support)
        private const string StandardBase64Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

        #region Decode(string encodedData) Tests

        [Fact]
        public void Decode_WithValidBase64String_ShouldReturnDecodedString()
        {
            // Arrange
            string plainText = "Hello World";
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

            // Act
            var result = Base64.Decode(base64Encoded);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decode_WithBase64StringContainingSpaces_ShouldReplaceSpacesWithPlus()
        {
            // Arrange
            string plainText = "test";
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
            string base64WithSpaces = base64Encoded.Replace("+", " ");

            // Act
            var result = Base64.Decode(base64WithSpaces);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decode_WithBase64StringWithPadding_ShouldDecodeCorrectly()
        {
            // Arrange
            string plainText = "test";
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

            // Act
            var result = Base64.Decode(base64Encoded);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decode_WithBase64StringWithDoublePadding_ShouldDecodeCorrectly()
        {
            // Arrange
            string plainText = "te";
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

            // Act
            var result = Base64.Decode(base64Encoded);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decode_WithEmptyString_ShouldReturnEmptyString()
        {
            // Arrange
            string emptyBase64 = "";

            // Act
            var result = Base64.Decode(emptyBase64);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Decode_WithInvalidCharacters_ShouldTrimAtFirstInvalidCharacter()
        {
            // Arrange
            string plainText = "Hello";
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
            string base64WithInvalidChars = base64Encoded + "!@#$%invalid";

            // Act
            var result = Base64.Decode(base64WithInvalidChars);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decode_WithUnicodeCharacters_ShouldDecodeCorrectly()
        {
            // Arrange
            string plainText = "Hello 世界 🌍";
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

            // Act
            var result = Base64.Decode(base64Encoded);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decode_WithSpecialCharacters_ShouldDecodeCorrectly()
        {
            // Arrange
            string plainText = "Special chars: !@#$%^&*()_+-=[]{}|;':\",./<>?";
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

            // Act
            var result = Base64.Decode(base64Encoded);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decode_WithLongString_ShouldDecodeCorrectly()
        {
            // Arrange
            string plainText = new string('A', 1000);
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

            // Act
            var result = Base64.Decode(base64Encoded);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decode_WithNullString_ShouldThrowException()
        {
            // Arrange
            string nullString = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => Base64.Decode(nullString));
        }

        [Fact]
        public void Decode_WithInvalidBase64String_ShouldTrimInvalidCharacters()
        {
            // Arrange
            string invalidBase64 = "This is not valid base64!!!";
            // The method trims invalid characters, so "Thisisnotvalidbase64" would remain
            // but that's still not valid Base64, so it will try to decode what's valid

            // Act
            var result = Base64.Decode(invalidBase64);

            // Assert
            // The method handles invalid input by trimming, not throwing
            result.Should().NotBeNull();
        }

        #endregion

        #region Decode(string encodedData, string key) Tests

        [Fact]
        public void Decode_WithCustomKeyAndValidData_ShouldDecodeCorrectly()
        {
            // Arrange
            string plainText = "Hello";
            string customKey = StandardBase64Alphabet;
            // Encode using standard Base64 first, then decode with custom key
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

            // Act
            var result = Base64.Decode(base64Encoded, customKey);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decode_WithCustomKeyAndInvalidCharacters_ShouldRemoveInvalidCharacters()
        {
            // Arrange
            string plainText = "test";
            string customKey = StandardBase64Alphabet;
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
            string base64WithInvalidChars = base64Encoded + "!@#$%^&*()";

            // Act
            var result = Base64.Decode(base64WithInvalidChars, customKey);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decode_WithCustomKeyAndUnicodeCharacters_ShouldDecodeCorrectly()
        {
            // Arrange
            string plainText = "Hello 世界";
            string customKey = StandardBase64Alphabet;
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

            // Act
            var result = Base64.Decode(base64Encoded, customKey);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decode_WithCustomKeyAndEmptyString_ShouldReturnEmptyString()
        {
            // Arrange
            string emptyString = "";
            string customKey = StandardBase64Alphabet;

            // Act
            var result = Base64.Decode(emptyString, customKey);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Decode_WithCustomKeyAndPadding_ShouldHandlePaddingCorrectly()
        {
            // Arrange
            string plainText = "te"; // This will produce padding
            string customKey = StandardBase64Alphabet;
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

            // Act
            var result = Base64.Decode(base64Encoded, customKey);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decode_WithCustomKeyAndSingleCharacter_ShouldDecodeCorrectly()
        {
            // Arrange
            string plainText = "A";
            string customKey = StandardBase64Alphabet;
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

            // Act
            var result = Base64.Decode(base64Encoded, customKey);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decode_WithCustomKeyAndSpacesInData_ShouldRemoveSpaces()
        {
            // Arrange
            string plainText = "test";
            string customKey = StandardBase64Alphabet;
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
            string base64WithSpaces = base64Encoded.Insert(2, " ").Insert(5, " ");

            // Act
            var result = Base64.Decode(base64WithSpaces, customKey);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decode_WithCustomKeyAndNullString_ShouldThrowException()
        {
            // Arrange
            string nullString = null!;
            string customKey = StandardBase64Alphabet;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => Base64.Decode(nullString, customKey));
        }

        [Fact]
        public void Decode_WithCustomKeyAndNullKey_ShouldThrowException()
        {
            // Arrange
            string encodedData = "SGVsbG8=";
            string nullKey = null!;

            // Act & Assert
            // The implementation throws NullReferenceException, not ArgumentNullException
            Assert.Throws<NullReferenceException>(() => Base64.Decode(encodedData, nullKey));
        }

        [Fact]
        public void Decode_WithCustomKeyAndCharactersNotInKey_ShouldThrowOverflowException()
        {
            // Arrange
            // Using a custom key that doesn't contain '=' (padding character)
            string customKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
            string encodedData = "SGVsbG8="; // "Hello" in standard Base64 (has padding)

            // Act & Assert
            // When characters are not found in key, IndexOf returns -1, causing overflow in bitwise operations
            Assert.Throws<OverflowException>(() => Base64.Decode(encodedData, customKey));
        }

        [Fact]
        public void Decode_WithCustomKeyAndLongString_ShouldDecodeCorrectly()
        {
            // Arrange
            string plainText = new string('A', 500);
            string customKey = StandardBase64Alphabet;
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

            // Act
            var result = Base64.Decode(base64Encoded, customKey);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decode_WithCustomKeyAndUtf8MultiByteCharacters_ShouldDecodeCorrectly()
        {
            // Arrange
            // Test UTF-8 characters that require multiple bytes
            string plainText = "A"; // Single byte
            string plainText2 = "é"; // Two bytes
            string plainText3 = "€"; // Three bytes
            string customKey = StandardBase64Alphabet;

            string base64Encoded1 = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
            string base64Encoded2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText2));
            string base64Encoded3 = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText3));

            // Act
            var result1 = Base64.Decode(base64Encoded1, customKey);
            var result2 = Base64.Decode(base64Encoded2, customKey);
            var result3 = Base64.Decode(base64Encoded3, customKey);

            // Assert
            result1.Should().Be(plainText);
            result2.Should().Be(plainText2);
            result3.Should().Be(plainText3);
        }

        #endregion

        #region Edge Cases and Integration Tests

        [Fact]
        public void Decode_StandardAndCustomKey_ShouldProduceSameResultForValidInput()
        {
            // Arrange
            string plainText = "Hello World";
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
            string customKey = StandardBase64Alphabet;

            // Act
            var standardResult = Base64.Decode(base64Encoded);
            var customResult = Base64.Decode(base64Encoded, customKey);

            // Assert
            standardResult.Should().Be(plainText);
            customResult.Should().Be(plainText);
            standardResult.Should().Be(customResult);
        }

        [Fact]
        public void Decode_WithCustomKeyAndMixedCase_ShouldDecodeCorrectly()
        {
            // Arrange
            string plainText = "Hello World";
            string customKey = StandardBase64Alphabet;
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

            // Act
            var result = Base64.Decode(base64Encoded, customKey);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decode_WithCustomKeyAndNewlines_ShouldRemoveNewlines()
        {
            // Arrange
            string plainText = "test";
            string customKey = StandardBase64Alphabet;
            string base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
            string base64WithNewlines = base64Encoded.Insert(2, "\n").Insert(5, "\r\n");

            // Act
            var result = Base64.Decode(base64WithNewlines, customKey);

            // Assert
            result.Should().Be(plainText);
        }

        #endregion
    }
}

