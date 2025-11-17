using Device.Fingerprint.Core;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Device.Fingerprint.Test.Core
{
    public class UtilitiesTests
    {
        #region GetURLFromDeviceCode Tests

        [Fact]
        public void GetURLFromDeviceCode_ShouldReturnNull_WhenInputIsNull()
        {
            // Act
            var result = Utilities.GetURLFromDeviceCode(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetURLFromDeviceCode_ShouldReturnNull_WhenInputIsEmpty()
        {
            // Act
            var result = Utilities.GetURLFromDeviceCode(string.Empty);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetURLFromDeviceCode_ShouldReturnNull_WhenInputIsWhitespace()
        {
            // Act
            var result = Utilities.GetURLFromDeviceCode("   ");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetURLFromDeviceCode_ShouldReturnUri_WhenValidUrlProvided()
        {
            // Arrange
            var deviceCode = "data1###data2###data3###data4###data5###data6###data7###https://example.com";

            // Act
            var result = Utilities.GetURLFromDeviceCode(deviceCode);

            // Assert
            result.Should().NotBeNull();
            result.ToString().Should().Be("https://example.com/");
        }

        [Fact]
        public void GetURLFromDeviceCode_ShouldReturnUri_WhenRelativeUrlProvided()
        {
            // Arrange
            var deviceCode = "data1###data2###data3###data4###data5###data6###data7###not-a-valid-url";

            // Act
            var result = Utilities.GetURLFromDeviceCode(deviceCode);

            // Assert - Uri.TryCreate accepts relative URIs
            result.Should().NotBeNull();
        }

        [Fact]
        public void GetURLFromDeviceCode_ShouldReturnUri_WhenLessThan8Parts()
        {
            // Arrange
            var deviceCode = "data1###data2###data3";

            // Act
            var result = Utilities.GetURLFromDeviceCode(deviceCode);

            // Assert - Returns Uri for empty string (valid relative URI)
            result.Should().NotBeNull();
        }

        #endregion

        #region GetUserAgentFromDeviceCode Tests

        [Fact]
        public void GetUserAgentFromDeviceCode_ShouldReturnEmpty_WhenInputIsNull()
        {
            // Act
            var result = Utilities.GetUserAgentFromDeviceCode(null);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetUserAgentFromDeviceCode_ShouldReturnUserAgent_WhenValidInput()
        {
            // Arrange
            var deviceCode = "data1###data2###data3###Mozilla/5.0";

            // Act
            var result = Utilities.GetUserAgentFromDeviceCode(deviceCode);

            // Assert
            result.Should().Be("Mozilla/5.0");
        }

        [Fact]
        public void GetUserAgentFromDeviceCode_ShouldReturnEmpty_WhenLessThan4Parts()
        {
            // Arrange
            var deviceCode = "data1###data2";

            // Act
            var result = Utilities.GetUserAgentFromDeviceCode(deviceCode);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region IsGuidDeviceId Tests

        [Fact]
        public void IsGuidDeviceId_ShouldReturnTrue_WhenValidGuid()
        {
            // Arrange
            var guid = Guid.NewGuid().ToString();

            // Act
            var result = Utilities.IsGuidDeviceId(guid);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsGuidDeviceId_ShouldReturnFalse_WhenInvalidGuid()
        {
            // Act
            var result = Utilities.IsGuidDeviceId("not-a-guid");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsGuidDeviceId_ShouldReturnFalse_WhenNull()
        {
            // Act
            var result = Utilities.IsGuidDeviceId(null);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsGuidDeviceId_ShouldReturnFalse_WhenEmpty()
        {
            // Act
            var result = Utilities.IsGuidDeviceId(string.Empty);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region GetDeviceCodeFromCapturedData Tests

        [Fact]
        public void GetDeviceCodeFromCapturedData_ShouldReturnDeviceCode_WhenValidInput()
        {
            // Arrange
            var capturedData = "d1###d2###d3###d4###d5###d6###DEVICE123";

            // Act
            var result = Utilities.GetDeviceCodeFromCapturedData(capturedData);

            // Assert
            result.Should().Be("DEVICE123");
        }

        [Fact]
        public void GetDeviceCodeFromCapturedData_ShouldReturnEmpty_WhenLessThan7Parts()
        {
            // Arrange
            var capturedData = "d1###d2###d3";

            // Act
            var result = Utilities.GetDeviceCodeFromCapturedData(capturedData);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region GetAnalysis Tests

        [Fact]
        public void GetAnalysis_ShouldReturnAnalysis_WhenValidInput()
        {
            // Arrange
            var data = "d1###d2###d3###d4###d5###d6###d7###d8###analysis-data";

            // Act
            var result = Utilities.GetAnalysis(data);

            // Assert
            result.Should().Be("analysis-data");
        }

        [Fact]
        public void GetAnalysis_ShouldReturnEmpty_WhenLessThan9Parts()
        {
            // Arrange
            var data = "d1###d2###d3";

            // Act
            var result = Utilities.GetAnalysis(data);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region Chunk Tests - Using Utilities extension method

        [Fact]
        public void ChunkExtension_ShouldSplitListIntoChunks_WhenValidInput()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // Act
            var chunks = Utilities.Chunk(numbers, 3).ToList();

            // Assert
            chunks.Should().HaveCount(4);
            chunks[0].Should().BeEquivalentTo(new[] { 1, 2, 3 });
            chunks[1].Should().BeEquivalentTo(new[] { 4, 5, 6 });
            chunks[2].Should().BeEquivalentTo(new[] { 7, 8, 9 });
            chunks[3].Should().BeEquivalentTo(new[] { 10 });
        }

        [Fact]
        public void ChunkExtension_ShouldReturnSingleChunk_WhenChunkSizeGreaterThanList()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3 };

            // Act
            var chunks = Utilities.Chunk(numbers, 10).ToList();

            // Assert
            chunks.Should().HaveCount(1);
            chunks[0].Should().BeEquivalentTo(new[] { 1, 2, 3 });
        }

        [Fact]
        public void ChunkExtension_ShouldReturnEmpty_WhenEmptyList()
        {
            // Arrange
            var numbers = new int[] { };

            // Act
            var chunks = Utilities.Chunk(numbers, 3).ToList();

            // Assert
            chunks.Should().BeEmpty();
        }

        [Fact]
        public void ChunkExtension_ShouldHandleChunkSize1_Correctly()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3 };

            // Act
            var chunks = Utilities.Chunk(numbers, 1).ToList();

            // Assert
            chunks.Should().HaveCount(3);
            chunks[0].Should().BeEquivalentTo(new[] { 1 });
            chunks[1].Should().BeEquivalentTo(new[] { 2 });
            chunks[2].Should().BeEquivalentTo(new[] { 3 });
        }

        #endregion

        #region Partition Tests

        [Fact]
        public void Partition_ShouldThrowArgumentNullException_WhenListIsNull()
        {
            // Act
            Action act = () => Utilities.Partition<int>(null, 5);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("list");
        }

        [Fact]
        public void Partition_ShouldThrowArgumentException_WhenTotalPartitionsIsZero()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3 };

            // Act
            Action act = () => Utilities.Partition(list, 0);

            // Assert
            act.Should().Throw<ArgumentException>().WithParameterName("totalPartitions");
        }

        [Fact]
        public void Partition_ShouldThrowArgumentException_WhenTotalPartitionsIsNegative()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3 };

            // Act
            Action act = () => Utilities.Partition(list, -1);

            // Assert
            act.Should().Throw<ArgumentException>().WithParameterName("totalPartitions");
        }

        [Fact]
        public void Partition_ShouldReturnCorrectPartitions_WhenEvenlyDivisible()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5, 6 };

            // Act
            var partitions = Utilities.Partition(list, 3).ToList();

            // Assert
            partitions.Should().HaveCount(3);
            partitions[0].Should().BeEquivalentTo(new[] { 1, 2 });
            partitions[1].Should().BeEquivalentTo(new[] { 3, 4 });
            partitions[2].Should().BeEquivalentTo(new[] { 5, 6 });
        }

        [Fact]
        public void Partition_ShouldReturnCorrectPartitions_WhenNotEvenlyDivisible()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5 };

            // Act
            var partitions = Utilities.Partition(list, 2).ToList();

            // Assert
            partitions.Should().HaveCount(2);
            partitions[0].Should().BeEquivalentTo(new[] { 1, 2, 3 });
            partitions[1].Should().BeEquivalentTo(new[] { 4, 5 });
        }

        #endregion

        #region XMLConvert Tests

        [Fact]
        public void XMLConvert_ShouldReturnXmlString_WhenValidObject()
        {
            // Arrange
            var testObject = new TestClass { Name = "Test", Value = 123 };

            // Act
            var result = Utilities.XMLConvert(testObject);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("<Name>Test</Name>");
            result.Should().Contain("<Value>123</Value>");
        }

        [Fact]
        public void XMLConvert_ShouldNotIncludeXmlDeclaration()
        {
            // Arrange
            var testObject = new TestClass { Name = "Test", Value = 123 };

            // Act
            var result = Utilities.XMLConvert(testObject);

            // Assert
            result.Should().NotContain("<?xml");
        }

        public class TestClass
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        #endregion
    }
}
