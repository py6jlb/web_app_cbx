using System;

namespace cookbook.DTOs.Common;

public interface ICollectionResponse<T>
{
    List<T> Items { get; init; }
}
