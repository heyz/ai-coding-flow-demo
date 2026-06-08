using Moq;
using SJ.BackEnd.Template.IRepository;
using SJ.BackEnd.Template.Services;

namespace SJ.BackEnd.Template.Tests.Services;

public class SysUserServiceTests
{
    [Fact]
    public async Task BatchDelete_WithValidIds_ReturnsCount()
    {
        // Arrange
        var ids = new long[] { 1, 2, 3 };
        var mockRepo = new Mock<IBaseRepository<SysUser>>();
        mockRepo.Setup(r => r.DeleteByIdsReturnCount(It.Is<object[]>(a => a.Length == 3)))
                .ReturnsAsync(3);

        var service = new SysUserService(mockRepo.Object);

        // Act
        var result = await service.BatchDelete(ids);

        // Assert
        Assert.Equal(3, result);
        mockRepo.Verify(r => r.DeleteByIdsReturnCount(It.Is<object[]>(a => a.Length == 3)), Times.Once);
    }

    [Fact]
    public async Task BatchDelete_WithEmptyIds_ReturnsZero()
    {
        // Arrange
        var ids = Array.Empty<long>();
        var mockRepo = new Mock<IBaseRepository<SysUser>>();
        mockRepo.Setup(r => r.DeleteByIdsReturnCount(It.IsAny<object[]>()))
                .ReturnsAsync(0);

        var service = new SysUserService(mockRepo.Object);

        // Act
        var result = await service.BatchDelete(ids);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task BatchDelete_WithNonExistentIds_ReturnsZero()
    {
        // Arrange
        var ids = new long[] { 999, 1000 };
        var mockRepo = new Mock<IBaseRepository<SysUser>>();
        mockRepo.Setup(r => r.DeleteByIdsReturnCount(It.Is<object[]>(a => a.Length == 2)))
                .ReturnsAsync(0);

        var service = new SysUserService(mockRepo.Object);

        // Act
        var result = await service.BatchDelete(ids);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task BatchDelete_WithMixedIds_ReturnsPartialCount()
    {
        // Arrange
        var ids = new long[] { 1, 2, 999 };
        var mockRepo = new Mock<IBaseRepository<SysUser>>();
        mockRepo.Setup(r => r.DeleteByIdsReturnCount(It.Is<object[]>(a => a.Length == 3)))
                .ReturnsAsync(2); // Only 2 of 3 exist

        var service = new SysUserService(mockRepo.Object);

        // Act
        var result = await service.BatchDelete(ids);

        // Assert
        Assert.Equal(2, result);
    }
}
