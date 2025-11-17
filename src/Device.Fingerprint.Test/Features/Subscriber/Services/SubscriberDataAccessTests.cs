using Device.Fingerprint.Features.Subscriber.Services;
using FluentAssertions;

namespace Device.Fingerprint.Test.Features.Subscriber.Services
{
    /// <summary>
    /// Unit tests for SubscriberDataAccess class.
    /// Note: SubscriberDataAccess initializes DynamicRepository in constructor,
    /// making it difficult to mock. These are basic instantiation tests.
    /// </summary>
    public class SubscriberDataAccessTests
    {
        [Fact]
        public void Constructor_ShouldInitializeRepository()
        {
            // Act
            var dataAccess = new SubscriberDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            dataAccess.Should().BeAssignableTo<ISubscriberDataAccess>();
        }

        [Fact]
        public void SubscriberDataAccess_ShouldImplementISubscriberDataAccessInterface()
        {
            // Arrange & Act
            var dataAccess = new SubscriberDataAccess();

            // Assert
            dataAccess.Should().BeAssignableTo<ISubscriberDataAccess>();
        }

        [Fact]
        public void GetSubscriberList_MethodSignature_ShouldReturnIEnumerable()
        {
            // Arrange
            var method = typeof(SubscriberDataAccess).GetMethod("GetSubscriberList");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Name.Should().Contain("IEnumerable");
        }

        [Fact]
        public void GetSubscriberByName_MethodSignature_ShouldReturnSubscriberEntity()
        {
            // Arrange
            var method = typeof(SubscriberDataAccess).GetMethod("GetSubscriberByName");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Name.Should().Be("SubscriberEntity");
        }
    }
}
