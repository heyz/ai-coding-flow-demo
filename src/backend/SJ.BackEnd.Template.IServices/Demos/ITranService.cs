#region  <<版本注释>>
/* ==============================================================================
// <copyright file="ITranService.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：ITranService
* 创 建 者：何应芝
* 创建时间：2026/5/26 9:54:52
* ==============================================================================*/
#endregion

namespace SJ.BackEnd.Template.IServices;

public interface ITranService
{
    Task<bool> TestTran();
}
