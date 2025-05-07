using System.ComponentModel;

using Microsoft.AspNetCore.Mvc;

namespace Api.Models.Category;

public record CategoryListOptions
{
    [FromQuery(Name = "pageIndex")]
    [DefaultValue(0)]
    public int PageIndex { get; init; }

    [FromQuery(Name = "pageSize")]
    [DefaultValue(10)]
    public int PageSize { get; init; }
}