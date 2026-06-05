#region  <<版本注释>>
/* ==============================================================================
// <copyright file="ConfigDbItem.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：ConfigDbItem
* 创 建 者：何应芝
* 创建时间：2026/5/25 17:22:52
* ==============================================================================*/
#endregion

namespace SJ.BackEnd.Template.Common.DB;

public class ConfigDbItem
{
    public string ConnId { get; set; }
    public DataBaseType DbType { get; set; }
    public bool Enabled { get; set; }
    public bool IsDefault { get; set; }
    public string Connection { get; set; }
}
