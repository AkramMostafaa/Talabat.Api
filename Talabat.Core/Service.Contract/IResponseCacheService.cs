﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Service.Contract
{
    public interface IResponseCacheService
    {
        Task CacheResponseAsync(string cacheKey,object response,TimeSpan timeToLive);

        Task<string?> GetCachedResponseAsync(string cacheKey);
    }
}