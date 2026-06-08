using System.Linq.Expressions;
using Moq;
using SJ.BackEnd.Template.IRepository;
using SJ.BackEnd.Template.Services;

namespace SJ.BackEnd.Template.Tests.Services;

public class SysUserRoleServiceTests
{
    private readonly Mock<IBaseRepository<SysUserRole>> _mockRepo;
    private readonly Mock<IBaseRepository<SysRole>> _mockRoleRepo;
    private readonly Mock<IBaseRepository<SysUser>> _mockUserRepo;
    private readonly ISysUserRoleService _service;

    public SysUserRoleServiceTests()
    {
        _mockRepo = new Mock<IBaseRepository<SysUserRole>>();
        _mockRoleRepo = new Mock<IBaseRepository<SysRole>>();
        _mockUserRepo = new Mock<IBaseRepository<SysUser>>();
        _service = new SysUserRoleService(_mockRepo.Object, _mockRoleRepo.Object, _mockUserRepo.Object);
    }

    [Fact]
    public async Task Bind_NewRelation_ReturnsTrue()
    {
        // Arrange
        _mockRepo.Setup(r => r.QueryByExpression(It.IsAny<Expression<Func<SysUserRole, bool>>>()))
                 .ReturnsAsync(new List<SysUserRole>());
        _mockRepo.Setup(r => r.Insert(It.IsAny<SysUserRole>()))
                 .ReturnsAsync(1L);

        // Act
        var result = await _service.Bind(1, 2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Bind_ExistingRelation_ReturnsFalse()
    {
        // Arrange
        _mockRepo.Setup(r => r.QueryByExpression(It.IsAny<Expression<Func<SysUserRole, bool>>>()))
                 .ReturnsAsync(new List<SysUserRole> { new SysUserRole { UserId = 1, RoleId = 2 } });

        // Act
        var result = await _service.Bind(1, 2);

        // Assert
        Assert.False(result);
        _mockRepo.Verify(r => r.Insert(It.IsAny<SysUserRole>()), Times.Never);
    }

    [Fact]
    public async Task Unbind_ExistingRelation_ReturnsTrue()
    {
        // Arrange
        var relation = new SysUserRole { Id = 1, UserId = 1, RoleId = 2 };
        _mockRepo.Setup(r => r.QueryByExpression(It.IsAny<Expression<Func<SysUserRole, bool>>>()))
                 .ReturnsAsync(new List<SysUserRole> { relation });
        _mockRepo.Setup(r => r.Delete(It.IsAny<SysUserRole>()))
                 .ReturnsAsync(true);

        // Act
        var result = await _service.Unbind(1, 2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Unbind_NonExistingRelation_ReturnsFalse()
    {
        // Arrange
        _mockRepo.Setup(r => r.QueryByExpression(It.IsAny<Expression<Func<SysUserRole, bool>>>()))
                 .ReturnsAsync(new List<SysUserRole>());

        // Act
        var result = await _service.Unbind(1, 2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetRolesByUserId_WithRelations_ReturnsRoles()
    {
        // Arrange
        _mockRepo.Setup(r => r.QueryByExpression(It.IsAny<Expression<Func<SysUserRole, bool>>>()))
                 .ReturnsAsync(new List<SysUserRole> { new SysUserRole { RoleId = 1 }, new SysUserRole { RoleId = 2 } });
        _mockRoleRepo.Setup(r => r.GetByIds(It.IsAny<object[]>()))
                     .ReturnsAsync(new List<SysRole> { new SysRole { Id = 1 }, new SysRole { Id = 2 } });

        // Act
        var roles = await _service.GetRolesByUserId(1);

        // Assert
        Assert.Equal(2, roles.Count);
    }

    [Fact]
    public async Task GetRolesByUserId_NoRelations_ReturnsEmpty()
    {
        // Arrange
        _mockRepo.Setup(r => r.QueryByExpression(It.IsAny<Expression<Func<SysUserRole, bool>>>()))
                 .ReturnsAsync(new List<SysUserRole>());

        // Act
        var roles = await _service.GetRolesByUserId(1);

        // Assert
        Assert.Empty(roles);
    }
}
