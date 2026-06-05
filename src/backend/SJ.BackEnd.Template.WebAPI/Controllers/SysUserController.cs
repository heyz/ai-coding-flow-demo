using Microsoft.AspNetCore.Mvc;
using SJ.BackEnd.Template.IServices;
using SJ.BackEnd.Template.Model;
using System.Linq.Expressions;

namespace SJ.BackEnd.Template.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SysUserController : ControllerBase
    {
        private readonly IBaseServices<SysUser> _userServices;
        private readonly ILogger<SysUserController> _logger;

        public SysUserController(IBaseServices<SysUser> userServices, ILogger<SysUserController> logger)
        {
            _userServices = userServices;
            _logger = logger;
        }

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
            try
            {
                Expression<Func<SysUser, bool>> whereExpression = _ => true;

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    whereExpression = u => u.RealName.Contains(keyword) || u.Nickname.Contains(keyword);
                }

                string orderByFields = "Id desc";
                var pageResult = await _userServices.GetPagedListByExpression(whereExpression, pageIndex, pageSize, orderByFields);

                return ApiResponse<PageModel<SysUser>>.Success("查询成功", pageResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查询用户列表失败");
                return ApiResponse<PageModel<SysUser>>.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 根据ID获取用户详情
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户信息</returns>
        [HttpGet("{id}")]
        public async Task<ApiResponse<SysUser>> GetById(long id)
        {
            try
            {
                var user = await _userServices.GetById(id);
                return ApiResponse<SysUser>.Success("获取成功", user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户详情失败，Id: {Id}", id);
                return ApiResponse<SysUser>.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>创建结果</returns>
        [HttpPost]
        public async Task<ApiResponse<long>> Create([FromBody] SysUser user)
        {
            try
            {
                user.Id = 0;
                user.CreatedTime = DateTime.Now;
                var result = await _userServices.Insert(user);
                return ApiResponse<long>.Success("创建成功", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建用户失败");
                return ApiResponse<long>.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="user">用户信息</param>
        /// <returns>更新结果</returns>
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> Update(long id, [FromBody] SysUser user)
        {
            try
            {
                user.Id = id;
                var result = await _userServices.Update(user);
                return ApiResponse<bool>.Success("更新成功", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新用户失败，Id: {Id}", id);
                return ApiResponse<bool>.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> Delete(long id)
        {
            try
            {
                var result = await _userServices.DeleteById(id);
                return ApiResponse<bool>.Success("删除成功", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除用户失败，Id: {Id}", id);
                return ApiResponse<bool>.Fail(ex.Message);
            }
        }
    }
}
