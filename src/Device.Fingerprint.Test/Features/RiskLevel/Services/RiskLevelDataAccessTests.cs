using Device.Fingerprint.Features.RiskLevel.Services;
using FluentAssertions;

namespace Device.Fingerprint.Test.Features.RiskLevel.Services
{
    /// <summary>
    /// Unit tests for RiskLevelDataAccess class.
    /// Note: RiskLevelDataAccess initializes DynamicRepository in constructor,
    /// making it difficult to mock. These are basic instantiation tests.
    /// </summary>
    public class RiskLevelDataAccessTests
    {
        [Fact]
        public void Constructor_ShouldInitializeRepository()
        {
            // Act
            var dataAccess = new RiskLevelDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            dataAccess.Should().BeAssignableTo<IRiskLevelDataAccess>();
        }

        [Fact]
        public void RiskLevelDataAccess_ShouldImplementIRiskLevelDataAccessInterface()
        {
            // Arrange & Act
            var dataAccess = new RiskLevelDataAccess();

            // Assert
            dataAccess.Should().BeAssignableTo<IRiskLevelDataAccess>();
        }

        [Fact]
        public void GetCustomerClassRiskLevel_MethodSignature_ShouldReturnIEnumerable()
        {
            // Arrange
            var method = typeof(RiskLevelDataAccess).GetMethod("GetCustomerClassRiskLevel");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Name.Should().Contain("IEnumerable");
        }

        [Fact]
        public void GetCustomerClassRiskLevelByAccount_MethodSignature_ShouldReturnIEnumerable()
        {
            // Arrange
            var method = typeof(RiskLevelDataAccess).GetMethod("GetCustomerClassRiskLevelByAccount");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Name.Should().Contain("IEnumerable");
        }
    }
}
