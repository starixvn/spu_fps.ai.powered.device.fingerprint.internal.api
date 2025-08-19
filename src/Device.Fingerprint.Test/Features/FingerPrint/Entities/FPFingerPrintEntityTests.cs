using Device.Fingerprint.Features.FingerPrint.Entities;
using FluentAssertions;

namespace Device.Fingerprint.Test.Features.FingerPrint.Entities
{
    public class FPFingerPrintEntityTests
    {
        [Fact]
        public void FPFingerPrintEntity_Properties_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var entity = new FPFingerPrintEntity();
            var testID = 12345UL;
            var testCode = "FP_TEST_CODE_001";
            var testAttribute = "test-attribute-fingerprint";
            var testInsertedTime = new DateTime(2024, 1, 15, 10, 30, 45);

            // Act
            entity.ID = testID;
            entity.Code = testCode;
            entity.Attribute = testAttribute;
            entity.InsertedTime = testInsertedTime;

            // Assert
            entity.ID.Should().Be(testID);
            entity.Code.Should().Be(testCode);
            entity.Attribute.Should().Be(testAttribute);
            entity.InsertedTime.Should().Be(testInsertedTime);
        }

        [Fact]
        public void FPFingerPrintEntity_DefaultValues_ShouldBeCorrect()
        {
            // Arrange & Act
            var entity = new FPFingerPrintEntity();

            // Assert
            entity.ID.Should().Be(0UL);
            entity.Code.Should().BeNull();
            entity.Attribute.Should().BeNull();
            entity.InsertedTime.Should().Be(default(DateTime));
        }

        [Fact]
        public void FPFingerPrintEntity_WithMaxValues_ShouldSetCorrectly()
        {
            // Arrange
            var entity = new FPFingerPrintEntity();
            var maxID = ulong.MaxValue;
            var maxDateTime = DateTime.MaxValue;

            // Act
            entity.ID = maxID;
            entity.InsertedTime = maxDateTime;

            // Assert
            entity.ID.Should().Be(maxID);
            entity.InsertedTime.Should().Be(maxDateTime);
        }

        [Fact]
        public void FPFingerPrintEntity_WithMinValues_ShouldSetCorrectly()
        {
            // Arrange
            var entity = new FPFingerPrintEntity();
            var minID = ulong.MinValue;
            var minDateTime = DateTime.MinValue;

            // Act
            entity.ID = minID;
            entity.InsertedTime = minDateTime;

            // Assert
            entity.ID.Should().Be(minID);
            entity.InsertedTime.Should().Be(minDateTime);
        }

        [Fact]
        public void FPFingerPrintEntity_Code_WithEmptyString_ShouldSetCorrectly()
        {
            // Arrange
            var entity = new FPFingerPrintEntity();

            // Act
            entity.Code = "";

            // Assert
            entity.Code.Should().Be("");
        }

        [Fact]
        public void FPFingerPrintEntity_Code_WithLongString_ShouldSetCorrectly()
        {
            // Arrange
            var entity = new FPFingerPrintEntity();
            var longCode = new string('A', 1000);

            // Act
            entity.Code = longCode;

            // Assert
            entity.Code.Should().Be(longCode);
        }

        [Fact]
        public void FPFingerPrintEntity_Attribute_WithComplexJson_ShouldSetCorrectly()
        {
            // Arrange
            var entity = new FPFingerPrintEntity();
            var complexAttribute = @"{""browser"":""Chrome"",""version"":""120.0"",""os"":""Windows"",""resolution"":""1920x1080""}";

            // Act
            entity.Attribute = complexAttribute;

            // Assert
            entity.Attribute.Should().Be(complexAttribute);
        }

        [Fact]
        public void FPFingerPrintEntity_Attribute_WithSpecialCharacters_ShouldSetCorrectly()
        {
            // Arrange
            var entity = new FPFingerPrintEntity();
            var specialAttribute = "Special chars: !@#$%^&*()_+-=[]{}|;':\"<>?,./";

            // Act
            entity.Attribute = specialAttribute;

            // Assert
            entity.Attribute.Should().Be(specialAttribute);
        }

        [Fact]
        public void FPFingerPrintEntity_InsertedTime_WithUtcNow_ShouldSetCorrectly()
        {
            // Arrange
            var entity = new FPFingerPrintEntity();
            var utcNow = DateTime.UtcNow;

            // Act
            entity.InsertedTime = utcNow;

            // Assert
            entity.InsertedTime.Should().BeCloseTo(utcNow, TimeSpan.FromMilliseconds(1));
        }

        [Fact]
        public void FPFingerPrintEntity_MultipleInstances_ShouldBeIndependent()
        {
            // Arrange
            var entity1 = new FPFingerPrintEntity();
            var entity2 = new FPFingerPrintEntity();

            // Act
            entity1.ID = 100;
            entity1.Code = "CODE1";
            entity2.ID = 200;
            entity2.Code = "CODE2";

            // Assert
            entity1.ID.Should().Be(100UL);
            entity1.Code.Should().Be("CODE1");
            entity2.ID.Should().Be(200UL);
            entity2.Code.Should().Be("CODE2");

            entity1.ID.Should().NotBe(entity2.ID);
            entity1.Code.Should().NotBe(entity2.Code);
        }

        [Fact]
        public void FPFingerPrintEntity_ObjectInitializer_ShouldWorkCorrectly()
        {
            // Arrange & Act
            var entity = new FPFingerPrintEntity
            {
                ID = 999UL,
                Code = "INIT_CODE",
                Attribute = "init-attribute",
                InsertedTime = new DateTime(2024, 6, 15)
            };

            // Assert
            entity.ID.Should().Be(999UL);
            entity.Code.Should().Be("INIT_CODE");
            entity.Attribute.Should().Be("init-attribute");
            entity.InsertedTime.Should().Be(new DateTime(2024, 6, 15));
        }
    }
}