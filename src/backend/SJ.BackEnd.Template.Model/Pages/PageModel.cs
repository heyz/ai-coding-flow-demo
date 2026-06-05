#region  <<版本注释>>
/* ==============================================================================
// <copyright file="PageModel.cs" company="Shiji.BO.CS">
// Copyright (c) SJ.BO.CS. All rights reserved.
// </copyright>
* 功能描述：PageModel
* 创 建 者：何应芝
* 创建时间：2026/5/25 15:26:18
* ==============================================================================*/
#endregion

using Mapster;

namespace SJ.BackEnd.Template.Model;

/// <summary>
/// 通用分页信息类
/// </summary>
public class PageModel<T>
{
    /// <summary>
    /// 当前页标
    /// </summary>
    public int Page { get; set; } = 1;
    /// <summary>
    /// 总页数
    /// </summary>
    public int PageCount => (int)Math.Ceiling((decimal)DataCount / PageSize);
    /// <summary>
    /// 数据总数
    /// </summary>
    public int DataCount { get; set; } = 0;
    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { set; get; } = 20;
    /// <summary>
    /// 返回数据
    /// </summary>
    public List<T>? Data { get; set; }

    public PageModel() { }

    public PageModel(int page, int dataCount, int pageSize, List<T> data)
    {
        this.Page = page;
        this.DataCount = dataCount;
        PageSize = pageSize;
        this.Data = data;
    }

    public PageModel<TOut> Create<TOut>()
    {
        return new PageModel<TOut>(Page, DataCount, PageSize, default!);
    }


    public PageModel<TOut> ConvertTo<TOut>()
    {
        var model = Create<TOut>();

        if (Data != null)
        {
            model.Data = Data.Adapt<List<TOut>>();
        }

        return model;
    }


    //public PageModel<TOut> ConvertTo<TOut>(IMapper mapper, Action<IMappingOperationOptions> options)
    //{
    //    var model = ConvertTo<TOut>();
    //    if (data != null)
    //    {
    //        model.data = mapper.Map<List<TOut>>(data, options);
    //    }

    //    return model;

    //}

}
