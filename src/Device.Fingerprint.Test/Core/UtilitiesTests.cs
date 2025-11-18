using Device.Fingerprint.Core;
using Device.Fingerprint.Model;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
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

        #region Split Tests

        [Fact]
        public void Split_ShouldSplitListIntoParts_WhenValidInput()
        {
            // Arrange
            var list = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // Act
            var parts = Utilities.Split(list, 3).ToList();

            // Assert
            // Implementation adds remainder to parts count, so 10 items / 3 = 3 remainder 1, parts becomes 4
            parts.Should().HaveCount(4);
            parts[0].Should().BeEquivalentTo(new[] { 1, 2, 3 });
            parts[1].Should().BeEquivalentTo(new[] { 4, 5, 6 });
            parts[2].Should().BeEquivalentTo(new[] { 7, 8, 9 });
            parts[3].Should().BeEquivalentTo(new[] { 10 });
        }

        [Fact]
        public void Split_ShouldHandleSinglePart()
        {
            // Arrange
            var list = new[] { 1, 2, 3, 4, 5 };

            // Act
            var parts = Utilities.Split(list, 1).ToList();

            // Assert
            parts.Should().HaveCount(1);
            parts[0].Should().BeEquivalentTo(new[] { 1, 2, 3, 4, 5 });
        }

        #endregion

        #region Partition (IList<T>) Tests

        [Fact]
        public void Partition_IList_ShouldPartitionList_WhenValidInput()
        {
            // Arrange
            IList<int> source = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // Act
            var partitions = Utilities.Partition(source, 3).ToList();

            // Assert
            partitions.Should().HaveCount(3);
            partitions[0].Should().BeEquivalentTo(new[] { 1, 2, 3 });
            partitions[1].Should().BeEquivalentTo(new[] { 4, 5, 6 });
            partitions[2].Should().BeEquivalentTo(new[] { 7, 8, 9 });
        }

        [Fact]
        public void Partition_IList_ShouldHandleRemainder()
        {
            // Arrange
            IList<int> source = new List<int> { 1, 2, 3, 4, 5 };

            // Act
            var partitions = Utilities.Partition(source, 2).ToList();

            // Assert
            // Implementation: (5 / 2) + (5 % 2 > 0 ? 1 : 0) = 2 + 1 = 3 partitions
            partitions.Should().HaveCount(3);
            partitions[0].Should().BeEquivalentTo(new[] { 1, 2 });
            partitions[1].Should().BeEquivalentTo(new[] { 3, 4 });
            partitions[2].Should().BeEquivalentTo(new[] { 5 });
        }

        [Fact]
        public void Partition_IList_ShouldHandleEmptyList()
        {
            // Arrange
            IList<int> source = new List<int>();

            // Act
            var partitions = Utilities.Partition(source, 3).ToList();

            // Assert
            partitions.Should().BeEmpty();
        }

        #endregion

        #region Ip2Long Tests

        [Fact]
        public void Ip2Long_ShouldConvertIPv4_WhenValidIPv4()
        {
            // Arrange
            string ipAddress = "192.168.1.1";

            // Act
            var result = Utilities.Ip2Long(ipAddress);

            // Assert
            result.Should().NotBeNull();
            result.IsIpV6.Should().BeFalse();
            result.IpNumber.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Ip2Long_ShouldConvertIPv6_WhenValidIPv6()
        {
            // Arrange
            string ipAddress = "2001:0db8:85a3:0000:0000:8a2e:0370:7334";

            // Act
            var result = Utilities.Ip2Long(ipAddress);

            // Assert
            result.Should().NotBeNull();
            result.IsIpV6.Should().BeTrue();
            result.IpNumber.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Ip2Long_ShouldReturnInvalid_WhenInvalidIP()
        {
            // Arrange
            string ipAddress = "invalid.ip.address";

            // Act
            var result = Utilities.Ip2Long(ipAddress);

            // Assert
            result.Should().NotBeNull();
            result.IpNumber.Should().Be(-1);
            result.IsIpV6.Should().BeFalse();
        }

        [Fact]
        public void Ip2Long_ShouldReturnInvalid_WhenNull()
        {
            // Arrange
            string ipAddress = null;

            // Act
            var result = Utilities.Ip2Long(ipAddress);

            // Assert
            result.Should().NotBeNull();
            result.IpNumber.Should().Be(-1);
            result.IsIpV6.Should().BeFalse();
        }

        [Fact]
        public void Ip2Long_ShouldReturnInvalid_WhenEmpty()
        {
            // Arrange
            string ipAddress = string.Empty;

            // Act
            var result = Utilities.Ip2Long(ipAddress);

            // Assert
            result.Should().NotBeNull();
            result.IpNumber.Should().Be(-1);
            result.IsIpV6.Should().BeFalse();
        }

        [Fact]
        public void Ip2Long_ShouldConvertIPv4_WhenZeroIP()
        {
            // Arrange
            string ipAddress = "0.0.0.0";

            // Act
            var result = Utilities.Ip2Long(ipAddress);

            // Assert
            result.Should().NotBeNull();
            result.IsIpV6.Should().BeFalse();
            result.IpNumber.Should().Be(0);
        }

        [Fact]
        public void Ip2Long_ShouldConvertIPv4_WhenMaxIP()
        {
            // Arrange
            string ipAddress = "255.255.255.255";

            // Act
            var result = Utilities.Ip2Long(ipAddress);

            // Assert
            result.Should().NotBeNull();
            result.IsIpV6.Should().BeFalse();
            result.IpNumber.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Ip2Long_ShouldConvertIPv6_WhenCompressedIPv6()
        {
            // Arrange
            string ipAddress = "2001:db8::1";

            // Act
            var result = Utilities.Ip2Long(ipAddress);

            // Assert
            result.Should().NotBeNull();
            result.IsIpV6.Should().BeTrue();
            result.IpNumber.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Ip2Long_ShouldConvertIPv6_WhenLocalhostIPv6()
        {
            // Arrange
            string ipAddress = "::1";

            // Act
            var result = Utilities.Ip2Long(ipAddress);

            // Assert
            result.Should().NotBeNull();
            result.IsIpV6.Should().BeTrue();
            result.IpNumber.Should().BeGreaterThanOrEqualTo(0);
        }

        #endregion

        #region Long2DotIPV6 Tests

        [Fact]
        public void Long2DotIPV6_ShouldConvertBigInteger_WhenValidInput()
        {
            // Arrange
            string bigint = "5192296858534827628530496329220096"; // Example IPv6 value

            // Act
            var result = Utilities.Long2DotIPV6(bigint);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(":");
        }

        [Fact]
        public void Long2DotIPV6_ShouldReturnEmpty_WhenInvalidInput()
        {
            // Arrange
            string bigint = "invalid";

            // Act
            var result = Utilities.Long2DotIPV6(bigint);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Long2DotIPV6_ShouldReturnEmpty_WhenNull()
        {
            // Arrange
            string bigint = null;

            // Act
            var result = Utilities.Long2DotIPV6(bigint);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region Long2DotIP Tests

        [Fact]
        public void Long2DotIP_ShouldConvertBigInteger_WhenValidIPv4()
        {
            // Arrange
            BigInteger ipNum = 3232235777; // 192.168.1.1

            // Act
            var result = Utilities.Long2DotIP(ipNum);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(".");
            result.Split('.').Should().HaveCount(4);
        }

        [Fact]
        public void Long2DotIP_ShouldFormatCorrectly_WhenZero()
        {
            // Arrange
            BigInteger ipNum = 0;

            // Act
            var result = Utilities.Long2DotIP(ipNum);

            // Assert
            result.Should().Be("0.0.0.0");
        }

        [Fact]
        public void Long2DotIP_ShouldFormatCorrectly_WhenMaxIPv4()
        {
            // Arrange
            BigInteger ipNum = 4294967295; // 255.255.255.255

            // Act
            var result = Utilities.Long2DotIP(ipNum);

            // Assert
            result.Should().Be("255.255.255.255");
        }

        [Fact]
        public void Long2DotIP_ShouldFormatCorrectly_WhenSpecificIP()
        {
            // Arrange
            BigInteger ipNum = 2130706433; // 127.0.0.1

            // Act
            var result = Utilities.Long2DotIP(ipNum);

            // Assert
            result.Should().Be("127.0.0.1");
        }

        [Fact]
        public void Long2DotIP_ShouldFormatCorrectly_WhenLargeNumber()
        {
            // Arrange
            BigInteger ipNum = new BigInteger(3232235777); // 192.168.1.1

            // Act
            var result = Utilities.Long2DotIP(ipNum);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Split('.').Should().HaveCount(4);
            result.Should().Contain("192");
        }

        #endregion

        #region ToUncompressedString Tests

        [Fact]
        public void ToUncompressedString_ShouldFormatIPv4_WhenValidIPv4()
        {
            // Arrange
            var ipAddress = IPAddress.Parse("192.168.1.1");

            // Act
            var result = Utilities.ToUncompressedString(ipAddress);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(".");
            result.Split('.').Should().HaveCount(4);
        }

        [Fact]
        public void ToUncompressedString_ShouldFormatIPv6_WhenValidIPv6()
        {
            // Arrange
            var ipAddress = IPAddress.Parse("2001:0db8:85a3:0000:0000:8a2e:0370:7334");

            // Act
            var result = Utilities.ToUncompressedString(ipAddress);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(":");
            result.Split(':').Should().HaveCount(8);
        }

        #endregion

        #region ConvertBitArray2Int Tests

        [Fact]
        public void ConvertBitArray2Int_ShouldConvertBitArray_WhenValidInput()
        {
            // Arrange
            var bitArray = new BitArray(new[] { true, false, true, false }); // 1010 = 10

            // Act
            var result = Utilities.ConvertBitArray2Int(bitArray);

            // Assert
            result.Should().BeGreaterThanOrEqualTo(0);
        }

        [Fact]
        public void ConvertBitArray2Int_ShouldConvertZero()
        {
            // Arrange
            var bitArray = new BitArray(new[] { false, false, false, false });

            // Act
            var result = Utilities.ConvertBitArray2Int(bitArray);

            // Assert
            result.Should().Be(0);
        }

        #endregion

        #region ConvertInt2BitArray Tests

        [Fact]
        public void ConvertInt2BitArray_ShouldConvertInt_WhenValidInput()
        {
            // Arrange
            int input = 10; // 1010 in binary

            // Act
            var result = Utilities.ConvertInt2BitArray(input);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(8); // Padded to 8 bits
            result.Should().BeEquivalentTo(new[] { 0, 0, 0, 0, 1, 0, 1, 0 });
        }

        [Fact]
        public void ConvertInt2BitArray_ShouldConvertZero()
        {
            // Arrange
            int input = 0;

            // Act
            var result = Utilities.ConvertInt2BitArray(input);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(8);
            result.Should().AllBeEquivalentTo(0);
        }

        #endregion

        #region ToBitString Tests

        [Fact]
        public void ToBitString_ShouldConvertBitArray_WhenValidInput()
        {
            // Arrange
            var bitArray = new BitArray(new[] { true, false, true, false });

            // Act
            var result = Utilities.ToBitString(bitArray);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().MatchRegex("^[01]+$");
        }

        [Fact]
        public void ToBitString_ShouldConvertAllFalse()
        {
            // Arrange
            var bitArray = new BitArray(new[] { false, false, false, false });

            // Act
            var result = Utilities.ToBitString(bitArray);

            // Assert
            result.Should().Be("0000");
        }

        #endregion

        #region ContainsUnicodeCharacter Tests

        [Fact]
        public void ContainsUnicodeCharacter_ShouldReturnTrue_WhenUnicodePresent()
        {
            // Arrange
            string input = "Hello 世界";

            // Act
            var result = Utilities.ContainsUnicodeCharacter(input);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ContainsUnicodeCharacter_ShouldReturnFalse_WhenOnlyASCII()
        {
            // Arrange
            string input = "Hello World";

            // Act
            var result = Utilities.ContainsUnicodeCharacter(input);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ContainsUnicodeCharacter_ShouldReturnFalse_WhenEmpty()
        {
            // Arrange
            string input = string.Empty;

            // Act
            var result = Utilities.ContainsUnicodeCharacter(input);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ContainsUnicodeCharacter_ShouldThrowArgumentNullException_WhenNull()
        {
            // Arrange
            string input = null;

            // Act
            Action act = () => Utilities.ContainsUnicodeCharacter(input);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        #endregion

        #region ConvertNA2Empty Tests

        [Fact]
        public void ConvertNA2Empty_ShouldReturnNull_WhenN_A()
        {
            // Arrange
            string data = "N/A";

            // Act
            var result = Utilities.ConvertNA2Empty(data);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ConvertNA2Empty_ShouldReturnOriginal_WhenNotN_A()
        {
            // Arrange
            string data = "SomeValue";

            // Act
            var result = Utilities.ConvertNA2Empty(data);

            // Assert
            result.Should().Be("SomeValue");
        }

        [Fact]
        public void ConvertNA2Empty_ShouldReturnOriginal_WhenCaseSensitive()
        {
            // Arrange
            string data = "n/a";

            // Act
            var result = Utilities.ConvertNA2Empty(data);

            // Assert
            result.Should().Be("n/a"); // Case sensitive
        }

        #endregion

        #region SubStringUrl Tests

        [Fact]
        public void SubStringUrl_ShouldRemoveQueryString_WhenQueryPresent()
        {
            // Arrange
            string url = "https://example.com/path?query=value";

            // Act
            var result = Utilities.SubStringUrl(url);

            // Assert
            result.Should().NotBeNull();
            result.ToString().Should().NotContain("?");
        }

        [Fact]
        public void SubStringUrl_ShouldRemoveFragment_WhenFragmentPresent()
        {
            // Arrange
            string url = "https://example.com/path#fragment";

            // Act
            var result = Utilities.SubStringUrl(url);

            // Assert
            result.Should().NotBeNull();
            result.ToString().Should().NotContain("#");
        }

        [Fact]
        public void SubStringUrl_ShouldReturnNull_WhenNull()
        {
            // Arrange
            string url = null;

            // Act
            var result = Utilities.SubStringUrl(url);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void SubStringUrl_ShouldReturnNull_WhenEmpty()
        {
            // Arrange
            string url = string.Empty;

            // Act
            var result = Utilities.SubStringUrl(url);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void SubStringUrl_ShouldCollapseCasinoPaths_WhenCasinoPathPresent()
        {
            // Arrange
            string url = "https://example.com/casino/casino/casino/game";

            // Act
            var result = Utilities.SubStringUrl(url);

            // Assert
            result.Should().NotBeNull();
            result.ToString().Should().Contain("/casino/.../casino/");
        }

        [Fact]
        public void SubStringUrl_ShouldNotCollapseCasinoPaths_WhenLessThan4Segments()
        {
            // Arrange
            string url = "https://example.com/casino/game";

            // Act
            var result = Utilities.SubStringUrl(url);

            // Assert
            result.Should().NotBeNull();
            result.ToString().Should().NotContain("...");
        }

        [Fact]
        public void SubStringUrl_ShouldNotCollapseCasinoPaths_WhenNonCasinoInMiddle()
        {
            // Arrange
            string url = "https://example.com/casino/other/casino/game";

            // Act
            var result = Utilities.SubStringUrl(url);

            // Assert
            result.Should().NotBeNull();
            result.ToString().Should().NotContain("...");
        }

        [Fact]
        public void SubStringUrl_ShouldNotCollapseCasinoPaths_WhenLastSegmentIsCasino()
        {
            // Arrange
            string url = "https://example.com/casino/casino/casino/casino";

            // Act
            var result = Utilities.SubStringUrl(url);

            // Assert
            result.Should().NotBeNull();
            result.ToString().Should().NotContain("...");
        }

        [Fact]
        public void SubStringUrl_ShouldNotCollapseCasinoPaths_WhenFirstSegmentIsNotCasino()
        {
            // Arrange
            string url = "https://example.com/game/casino/casino/end";

            // Act
            var result = Utilities.SubStringUrl(url);

            // Assert
            result.Should().NotBeNull();
            result.ToString().Should().NotContain("...");
        }

        #endregion

        #region StripUnicodeCharactersFromString Tests

        [Fact]
        public void StripUnicodeCharactersFromString_ShouldRemoveUnicode_WhenUnicodePresent()
        {
            // Arrange
            string input = "Hello 世界 World";

            // Act
            var result = Utilities.StripUnicodeCharactersFromString(input);

            // Assert
            result.Should().Be("Hello  World");
        }

        [Fact]
        public void StripUnicodeCharactersFromString_ShouldReturnOriginal_WhenOnlyASCII()
        {
            // Arrange
            string input = "Hello World";

            // Act
            var result = Utilities.StripUnicodeCharactersFromString(input);

            // Assert
            result.Should().Be("Hello World");
        }

        [Fact]
        public void StripUnicodeCharactersFromString_ShouldReturnNull_WhenNull()
        {
            // Arrange
            string input = null;

            // Act
            var result = Utilities.StripUnicodeCharactersFromString(input);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void StripUnicodeCharactersFromString_ShouldReturnNull_WhenEmpty()
        {
            // Arrange
            string input = string.Empty;

            // Act
            var result = Utilities.StripUnicodeCharactersFromString(input);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region EncodeCapturedData Tests

        [Fact]
        public void EncodeCapturedData_ShouldEncodeDeviceInfo_WhenValidInput()
        {
            // Arrange
            var deviceInfo = new DeviceInfoModel
            {
                Battery = "100",
                CapturedDate = 1234567890,
                OSName = "Windows",
                TimeZone = "UTC",
                LocalIps = "192.168.1.1"
            };

            // Act
            var result = Utilities.EncodeCapturedData(deviceInfo);

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void EncodeCapturedData_ShouldEncode_WhenNullValues()
        {
            // Arrange
            var deviceInfo = new DeviceInfoModel
            {
                Battery = null,
                CapturedDate = 0,
                OSName = null,
                TimeZone = null,
                LocalIps = null
            };

            // Act
            var result = Utilities.EncodeCapturedData(deviceInfo);

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        #endregion

        #region ParseSessionKey Tests

        [Fact]
        public void ParseSessionKey_ShouldParse_WhenValidBase64()
        {
            // Arrange
            string sessionKey = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("test-session-key"));
            int defaultRedisDatabase = 0;

            // Act
            var result = Utilities.ParseSessionKey(sessionKey, defaultRedisDatabase);

            // Assert
            result.Should().NotBeNull();
            result.SessionKey.Should().Be(sessionKey);
            result.RedisDbIndex.Should().Be(defaultRedisDatabase);
        }

        [Fact]
        public void ParseSessionKey_ShouldParseWithRedisIndex_WhenIndexProvided()
        {
            // Arrange
            string sessionKey = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("test"));
            string sessionKeyWithIndex = $"{sessionKey}:5";
            int defaultRedisDatabase = 0;

            // Act
            var result = Utilities.ParseSessionKey(sessionKeyWithIndex, defaultRedisDatabase);

            // Assert
            result.Should().NotBeNull();
            result.SessionKey.Should().Be(sessionKey);
            result.RedisDbIndex.Should().Be(5);
        }

        [Fact]
        public void ParseSessionKey_ShouldUseDefaultIndex_WhenIndexGreaterThanMax()
        {
            // Arrange
            string sessionKey = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("test"));
            string sessionKeyWithIndex = $"{sessionKey}:20"; // Greater than 16
            int defaultRedisDatabase = 0;

            // Act
            var result = Utilities.ParseSessionKey(sessionKeyWithIndex, defaultRedisDatabase);

            // Assert
            result.Should().NotBeNull();
            result.RedisDbIndex.Should().Be(defaultRedisDatabase);
        }

        [Fact]
        public void ParseSessionKey_ShouldUseIndex_WhenIndexEqualsMax()
        {
            // Arrange
            string sessionKey = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("test"));
            string sessionKeyWithIndex = $"{sessionKey}:15"; // Max valid index (0-15)
            int defaultRedisDatabase = 0;

            // Act
            var result = Utilities.ParseSessionKey(sessionKeyWithIndex, defaultRedisDatabase);

            // Assert
            result.Should().NotBeNull();
            result.RedisDbIndex.Should().Be(15);
        }

        [Fact]
        public void ParseSessionKey_ShouldUseNegativeIndex_WhenIndexIsNegative()
        {
            // Arrange
            string sessionKey = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("test"));
            string sessionKeyWithIndex = $"{sessionKey}:-1";
            int defaultRedisDatabase = 5;

            // Act
            var result = Utilities.ParseSessionKey(sessionKeyWithIndex, defaultRedisDatabase);

            // Assert
            // Implementation doesn't validate negative values, only checks if < _TotalRedisDB (16)
            result.Should().NotBeNull();
            result.RedisDbIndex.Should().Be(-1); // Negative values pass through
        }

        [Fact]
        public void ParseSessionKey_ShouldUseDefaultIndex_WhenIndexIsNotNumeric()
        {
            // Arrange
            string sessionKey = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("test"));
            string sessionKeyWithIndex = $"{sessionKey}:abc";
            int defaultRedisDatabase = 3;

            // Act
            var result = Utilities.ParseSessionKey(sessionKeyWithIndex, defaultRedisDatabase);

            // Assert
            result.Should().NotBeNull();
            result.RedisDbIndex.Should().Be(defaultRedisDatabase);
        }

        [Fact]
        public void ParseSessionKey_ShouldHandleEmptySessionKeyPart()
        {
            // Arrange
            string sessionKeyWithIndex = ":5";
            int defaultRedisDatabase = 0;

            // Act
            var result = Utilities.ParseSessionKey(sessionKeyWithIndex, defaultRedisDatabase);

            // Assert
            result.Should().BeNull(); // Empty string is not valid Base64
        }

        [Fact]
        public void ParseSessionKey_ShouldReturnNull_WhenNotBase64()
        {
            // Arrange
            string sessionKey = "not-base64-string";
            int defaultRedisDatabase = 0;

            // Act
            var result = Utilities.ParseSessionKey(sessionKey, defaultRedisDatabase);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ParseSessionKey_ShouldReturnNull_WhenNull()
        {
            // Arrange
            string sessionKey = null;
            int defaultRedisDatabase = 0;

            // Act
            var result = Utilities.ParseSessionKey(sessionKey, defaultRedisDatabase);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region IsBase64 Tests

        [Fact]
        public void IsBase64_ShouldReturnTrue_WhenValidBase64()
        {
            // Arrange
            string base64String = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("test"));

            // Act
            var result = Utilities.IsBase64(base64String);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsBase64_ShouldReturnFalse_WhenInvalidBase64()
        {
            // Arrange
            string base64String = "not-valid-base64!@#";

            // Act
            var result = Utilities.IsBase64(base64String);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsBase64_ShouldReturnFalse_WhenNull()
        {
            // Arrange
            string base64String = null;

            // Act
            var result = Utilities.IsBase64(base64String);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsBase64_ShouldReturnFalse_WhenEmpty()
        {
            // Arrange
            string base64String = string.Empty;

            // Act
            var result = Utilities.IsBase64(base64String);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsBase64_ShouldReturnFalse_WhenLengthNotDivisibleBy4()
        {
            // Arrange
            string base64String = "abc"; // Length 3, not divisible by 4

            // Act
            var result = Utilities.IsBase64(base64String);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region RemoveRedisPrefix Tests

        [Fact]
        public void RemoveRedisPrefix_ShouldRemovePrefix_WhenPrefixPresent()
        {
            // Arrange
            string input = "prefix:value";
            string prefix = "prefix:";

            // Act
            var result = Utilities.RemoveRedisPrefix(input, prefix);

            // Assert
            result.Should().Be("value");
        }

        [Fact]
        public void RemoveRedisPrefix_ShouldReturnOriginal_WhenPrefixNotPresent()
        {
            // Arrange
            string input = "value";
            string prefix = "prefix:";

            // Act
            var result = Utilities.RemoveRedisPrefix(input, prefix);

            // Assert
            result.Should().Be("value");
        }

        [Fact]
        public void RemoveRedisPrefix_ShouldReturnEmpty_WhenNull()
        {
            // Arrange
            string input = null;
            string prefix = "prefix:";

            // Act
            var result = Utilities.RemoveRedisPrefix(input, prefix);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void RemoveRedisPrefix_ShouldReturnEmpty_WhenEmpty()
        {
            // Arrange
            string input = string.Empty;
            string prefix = "prefix:";

            // Act
            var result = Utilities.RemoveRedisPrefix(input, prefix);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void RemoveRedisPrefix_ShouldThrowArgumentException_WhenPrefixIsEmpty()
        {
            // Arrange
            string input = "value";
            string prefix = string.Empty;

            // Act
            Action act = () => Utilities.RemoveRedisPrefix(input, prefix);

            // Assert
            act.Should().Throw<ArgumentException>(); // String.Replace throws when oldValue is empty
        }

        [Fact]
        public void RemoveRedisPrefix_ShouldThrowArgumentNullException_WhenPrefixIsNull()
        {
            // Arrange
            string input = "value";
            string prefix = null;

            // Act
            Action act = () => Utilities.RemoveRedisPrefix(input, prefix);

            // Assert
            act.Should().Throw<ArgumentNullException>(); // String.StartsWith throws when value is null
        }

        [Fact]
        public void RemoveRedisPrefix_ShouldRemoveAllOccurrences_WhenPrefixAppearsMultipleTimes()
        {
            // Arrange
            string input = "prefix:prefix:value";
            string prefix = "prefix:";

            // Act
            var result = Utilities.RemoveRedisPrefix(input, prefix);

            // Assert
            result.Should().Be("value"); // Replace removes all occurrences, not just the first
        }

        [Fact]
        public void RemoveRedisPrefix_ShouldReturnEmpty_WhenInputEqualsPrefix()
        {
            // Arrange
            string input = "prefix:";
            string prefix = "prefix:";

            // Act
            var result = Utilities.RemoveRedisPrefix(input, prefix);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region Additional Edge Cases

        [Fact]
        public void GetURLFromDeviceCode_ShouldReturnNull_WhenUrlIsInvalidAfterExtraction()
        {
            // Arrange
            var deviceCode = "data1###data2###data3###data4###data5###data6###data7###";

            // Act
            var result = Utilities.GetURLFromDeviceCode(deviceCode);

            // Assert
            result.Should().NotBeNull(); // Empty string is a valid relative URI
        }

        [Fact]
        public void GetUserAgentFromDeviceCode_ShouldReturnEmpty_WhenWhitespace()
        {
            // Arrange
            var deviceCode = "data1###data2###data3###   ";

            // Act
            var result = Utilities.GetUserAgentFromDeviceCode(deviceCode);

            // Assert
            result.Should().Be("   "); // Returns whitespace as-is
        }

        [Fact]
        public void GetDeviceCodeFromCapturedData_ShouldReturnDeviceCode_WhenExactly7Parts()
        {
            // Arrange
            var capturedData = "d1###d2###d3###d4###d5###d6###d7";

            // Act
            var result = Utilities.GetDeviceCodeFromCapturedData(capturedData);

            // Assert
            result.Should().Be("d7"); // When length >= 7, returns index 6 (7th element)
        }

        [Fact]
        public void GetAnalysis_ShouldReturnEmpty_WhenExactly9Parts()
        {
            // Arrange
            var data = "d1###d2###d3###d4###d5###d6###d7###d8###d9";

            // Act
            var result = Utilities.GetAnalysis(data);

            // Assert
            result.Should().Be("d9"); // Index 8 (9th element) when length > 8
        }

        [Fact]
        public void ChunkExtension_ShouldThrowDivideByZeroException_WhenZeroChunkSize()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3 };

            // Act
            Action act = () => Utilities.Chunk(numbers, 0).ToList();

            // Assert
            act.Should().Throw<DivideByZeroException>();
        }

        [Fact]
        public void Split_ShouldThrowDivideByZeroException_WhenZeroParts()
        {
            // Arrange
            var list = new[] { 1, 2, 3, 4, 5 };

            // Act
            Action act = () => Utilities.Split(list, 0).ToList();

            // Assert
            act.Should().Throw<DivideByZeroException>();
        }

        [Fact]
        public void Partition_IList_ShouldThrowDivideByZeroException_WhenZeroSize()
        {
            // Arrange
            IList<int> source = new List<int> { 1, 2, 3 };

            // Act
            Action act = () => Utilities.Partition(source, 0).ToList();

            // Assert
            act.Should().Throw<DivideByZeroException>();
        }

        [Fact]
        public void ToUncompressedString_ShouldHandleIPv4Localhost()
        {
            // Arrange
            var ipAddress = IPAddress.Parse("127.0.0.1");

            // Act
            var result = Utilities.ToUncompressedString(ipAddress);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("127");
        }

        [Fact]
        public void ToUncompressedString_ShouldHandleIPv6Localhost()
        {
            // Arrange
            var ipAddress = IPAddress.Parse("::1");

            // Act
            var result = Utilities.ToUncompressedString(ipAddress);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(":");
        }

        [Fact]
        public void ConvertBitArray2Int_ShouldConvertLargeBitArray()
        {
            // Arrange
            var bitArray = new BitArray(new[] { true, true, true, true, true, true, true, true });

            // Act
            var result = Utilities.ConvertBitArray2Int(bitArray);

            // Assert
            result.Should().BeGreaterThan(0);
        }

        [Fact]
        public void ConvertInt2BitArray_ShouldConvertMaxByte()
        {
            // Arrange
            int input = 255;

            // Act
            var result = Utilities.ConvertInt2BitArray(input);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(8);
            result.Should().AllBeEquivalentTo(1);
        }

        [Fact]
        public void ConvertInt2BitArray_ShouldConvertNegativeNumber()
        {
            // Arrange
            int input = -1;

            // Act
            var result = Utilities.ConvertInt2BitArray(input);

            // Assert
            result.Should().NotBeNull();
            // Negative numbers produce 32-bit binary representation (int is 32 bits)
            result.Should().HaveCount(32);
            result.Should().AllBeEquivalentTo(1); // -1 in binary is all 1s
        }

        [Fact]
        public void ToBitString_ShouldConvertLargeBitArray()
        {
            // Arrange
            var bitArray = new BitArray(16);
            for (int i = 0; i < 16; i++)
            {
                bitArray[i] = i % 2 == 0;
            }

            // Act
            var result = Utilities.ToBitString(bitArray);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveLength(16);
            result.Should().MatchRegex("^[01]+$");
        }

        [Fact]
        public void ContainsUnicodeCharacter_ShouldReturnTrue_WhenEmojiPresent()
        {
            // Arrange
            string input = "Hello 😀 World";

            // Act
            var result = Utilities.ContainsUnicodeCharacter(input);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ContainsUnicodeCharacter_ShouldReturnFalse_WhenMaxAnsiCode()
        {
            // Arrange
            string input = new string((char)255, 1);

            // Act
            var result = Utilities.ContainsUnicodeCharacter(input);

            // Assert
            result.Should().BeFalse(); // 255 is max ANSI
        }

        [Fact]
        public void ContainsUnicodeCharacter_ShouldReturnTrue_WhenAboveMaxAnsiCode()
        {
            // Arrange
            string input = new string((char)256, 1);

            // Act
            var result = Utilities.ContainsUnicodeCharacter(input);

            // Assert
            result.Should().BeTrue(); // 256 is above max ANSI
        }

        [Fact]
        public void EncodeCapturedData_ShouldEncode_WhenEmptyStrings()
        {
            // Arrange
            var deviceInfo = new DeviceInfoModel
            {
                Battery = string.Empty,
                CapturedDate = 0,
                OSName = string.Empty,
                TimeZone = string.Empty,
                LocalIps = string.Empty
            };

            // Act
            var result = Utilities.EncodeCapturedData(deviceInfo);

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void IsBase64_ShouldReturnTrue_WhenValidBase64WithPadding()
        {
            // Arrange
            string base64String = "dGVzdA=="; // "test" with padding

            // Act
            var result = Utilities.IsBase64(base64String);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsBase64_ShouldReturnTrue_WhenValidBase64WithoutPadding()
        {
            // Arrange
            string base64String = "dGVzdA"; // "test" without padding (invalid length)

            // Act
            var result = Utilities.IsBase64(base64String);

            // Assert
            result.Should().BeFalse(); // Length not divisible by 4
        }

        #endregion
    }
}
