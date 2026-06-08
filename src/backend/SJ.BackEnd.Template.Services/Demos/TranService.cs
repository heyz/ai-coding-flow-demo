#region  <<版本注释>>
/* ==============================================================================
// <copyright file="TranService.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：TranService
* 创 建 者：何应芝
* 创建时间：2026/5/26 9:55:53
* ==============================================================================*/
#endregion

namespace SJ.BackEnd.Template.Services;

public class TranService(IUnitOfWorkManage db, IBaseRepository<LlmConfig> configRepo) : ITranService
{
    private readonly IUnitOfWorkManage _uow = db;
    private readonly IBaseRepository<LlmConfig> _configRepo = configRepo;

    public async Task<bool> TestTran01()
    {
        try
        {
            _uow.BeginTran();

            var client = _uow.GetDbClient();


            var config = new LlmConfig
            {
                Provider = "aliyun"
            };

            await client.Insertable(config).ExecuteCommandAsync();

            // throw new Exception("测试事务01");

            var role = new SysRole
            {
                Name = "测试事务02",
                Code = "test02"
            };

            await client.GetConnectionScope("2").Insertable(role).ExecuteCommandAsync();

            await TestTran02();

            _uow.CommitTran();

            return true;
        }
        catch (Exception)
        {
            _uow.RollbackTran();
            throw;
        }

    }


    private async Task<bool> TestTran02()
    {
        try
        {
            _uow.BeginTran();

            var client = _uow.GetDbClient();

            var config = new LlmConfig
            {
                Provider = "tengx"
            };

            await client.Insertable(config).ExecuteCommandAsync();

            // throw new Exception("测试事务01");

            var role = new SysRole
            {
                Name = "测试事务03",
                Code = "test03"
            };

            await client.GetConnectionScope("2").Insertable(role).ExecuteCommandAsync();

            throw new Exception("测试事务03-抛错");

            _uow.CommitTran();

            return true;
        }
        catch (Exception)
        {
            _uow.RollbackTran();
            // throw  new Exception("03") ;
            return false;
        }

    }

    public async Task<bool> TestTran()
    {
        return await TestTran01();
        return false;

        try
        {
            _uow.BeginTran();

            var client = _uow.GetDbClient();

            var config = new LlmConfig
            {
                Provider = "aliyun"
            };

            await client.Insertable(config).ExecuteCommandAsync();

            // throw new Exception("测试事务01");

            var role = new SysRole
            {
                Name = "测试事务02",
                Code = "test02"
            };

            await client.Insertable(role).ExecuteCommandAsync();

            //throw new Exception("测试事务02");


            _uow.CommitTran();

            return true;
        }
        catch (Exception)
        {
            _uow.RollbackTran();

            return false;
        }

    }
}
