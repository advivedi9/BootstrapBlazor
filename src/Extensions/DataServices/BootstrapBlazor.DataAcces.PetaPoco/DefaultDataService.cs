﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Components;
using PetaPoco;
using PetaPoco.Extensions;

namespace BootstrapBlazor.DataAcces.PetaPoco;

/// <summary>
/// PetaPoco ORM 的 IDataService 接口实现
/// </summary>
internal class DefaultDataService<TModel> : DataServiceBase<TModel> where TModel : class, new()
{
    private readonly IDatabase _db;

    /// <summary>
    /// 构造函数
    /// </summary>
    public DefaultDataService(IDatabase db)
    {
        _db = db;
    }

    /// <summary>
    /// 删除方法
    /// </summary>
    /// <param name="models"></param>
    /// <returns></returns>
    public override Task<bool> DeleteAsync(IEnumerable<TModel> models)
    {
        // 通过模型获取主键列数据
        // 支持批量删除
        _db.DeleteBatch(models);
        return Task.FromResult(true);
    }

    /// <summary>
    /// 保存方法
    /// </summary>
    /// <param name="model"></param>
    /// <param name="changedType"></param>
    /// <returns></returns>
    public override async Task<bool> SaveAsync(TModel model, ItemChangedType changedType)
    {
        await _db.SaveAsync(model);
        return true;
    }

    /// <summary>
    /// 查询方法
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    public override async Task<QueryData<TModel>> QueryAsync(QueryPageOptions option)
    {
        var ret = new QueryData<TModel>()
        {
            IsSorted = true,
            IsFiltered = true,
            IsSearch = true
        };

        if (option.IsPage)
        {
            var items = await _db.PageAsync<TModel>(option.PageIndex, option.PageItems, option.Filters.Concat(option.Searchs), option.SortName, option.SortOrder);

            ret.TotalCount = int.Parse(items.TotalItems.ToString());
            ret.Items = items.Items;
        }
        else
        {
            var items = await _db.FetchAsync<TModel>(option.Filters.Concat(option.Searchs), option.SortName, option.SortOrder);
            ret.TotalCount = items.Count;
            ret.Items = items;
        }
        return ret;
    }
}
