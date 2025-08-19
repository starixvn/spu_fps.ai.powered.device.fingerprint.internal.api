using Device.Fingerprint.Features.Transaction.Criterias;
using FluentAssertions;

namespace Device.Fingerprint.Test.Features.Transaction.Criterias
{
    public class DeviceIdentifyNoFingerprintCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ShouldReturnCorrectKey()
        {
            // Arrange
            var criteria = new DeviceIdentifyNoFingerprintCriteria();

            // Act
            var result = criteria.GetSettingKey();

            // Assert
            result.Should().Be("FPS_FP_Transform_DeviceIdentify_NoFingerPrint");
        }

        [Fact]
        public void IsValid_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var criteria = new DeviceIdentifyNoFingerprintCriteria();

            // Act
            var result = criteria.IsValid();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void TransList_Property_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var criteria = new DeviceIdentifyNoFingerprintCriteria();
            var testTransList = "1,2,3,4,5";

            // Act
            criteria.TransList = testTransList;

            // Assert
            criteria.TransList.Should().Be(testTransList);
        }

        [Fact]
        public void TransList_WithEmptyString_ShouldSetCorrectly()
        {
            // Arrange
            var criteria = new DeviceIdentifyNoFingerprintCriteria();
            var emptyTransList = "";

            // Act
            criteria.TransList = emptyTransList;

            // Assert
            criteria.TransList.Should().Be(emptyTransList);
        }

        [Fact]
        public void TransList_WithNull_ShouldSetCorrectly()
        {
            // Arrange
            var criteria = new DeviceIdentifyNoFingerprintCriteria();

            // Act
            criteria.TransList = null;

            // Assert
            criteria.TransList.Should().BeNull();
        }

        [Fact]
        public void TransList_WithLargeString_ShouldSetCorrectly()
        {
            // Arrange
            var criteria = new DeviceIdentifyNoFingerprintCriteria();
            var largeTransList = string.Join(",", Enumerable.Range(1, 1000));

            // Act
            criteria.TransList = largeTransList;

            // Assert
            criteria.TransList.Should().Be(largeTransList);
        }

        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            // Arrange & Act
            var criteria = new DeviceIdentifyNoFingerprintCriteria();

            // Assert
            criteria.TransList.Should().BeNull();
            criteria.IsValid().Should().BeTrue();
            criteria.GetSettingKey().Should().Be("FPS_FP_Transform_DeviceIdentify_NoFingerPrint");
        }

        [Fact]
        public void TransList_WithSpecialCharacters_ShouldSetCorrectly()
        {
            // Arrange
            var criteria = new DeviceIdentifyNoFingerprintCriteria();
            var specialCharTransList = "1,2;3|4&5%6@7#8$9";

            // Act
            criteria.TransList = specialCharTransList;

            // Assert
            criteria.TransList.Should().Be(specialCharTransList);
        }
    }
}