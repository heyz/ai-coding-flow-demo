using Microsoft.AspNetCore.Mvc;
using SJ.BackEnd.Template.IServices;
using SJ.BackEnd.Template.Model;
using SJ.BackEnd.Template.Model.Dtos.SysUser;

namespace SJ.BackEnd.Template.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class SysUserController(ISysUserService sysUserService) : ControllerBase
{
    private readonly ISysUserService _sysUserService = sysUserService;

    /// <summary>
    /// 获取用户列表（分页）
    /// </summary>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <param name="keyword">搜索关键词（真实姓名/昵称）</param>
    /// <returns>分页结果</returns>
    [HttpGet("list")]
    public async Task<ApiResponse<PageModel<SysUser>>> GetList([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? keyword = null)
    {
        var pageResult = await _sysUserService.GetPagedList(pageIndex, pageSize, keyword);
        return ApiResponse<PageModel<SysUser>>.Success("查询成功", pageResult);
    }

    /// <summary>
    /// 根据ID获取用户详情
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户信息</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<SysUser>> GetById(long id)
    {
        var user = await _sysUserService.GetById(id);
        return ApiResponse<SysUser>.Success("获取成功", user);
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="request">创建用户请求</param>
    /// <returns>创建结果</returns>
    [HttpPost]
    public async Task<ApiResponse<CreateUserResponse>> Create([FromBody] CreateUserRequest request)
    {
        var result = await _sysUserService.Create(request);
        if (result == null)
            return ApiResponse<CreateUserResponse>.Fail("用户昵称已存在");
        return ApiResponse<CreateUserResponse>.Success("创建成功", result);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="request">修改用户请求</param>
    /// <returns>更新结果</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(long id, [FromBody] UpdateUserRequest request)
    {
        var result = await _sysUserService.Update(id, request);
        if (!result)
            return ApiResponse<bool>.Fail("用户昵称已存在");
        return ApiResponse<bool>.Success("更新成功", true);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        var result = await _sysUserService.Delete(id);
        return ApiResponse<bool>.Success("删除成功", result);
    }
}
