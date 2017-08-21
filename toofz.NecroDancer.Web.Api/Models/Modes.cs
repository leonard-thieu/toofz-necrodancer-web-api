﻿using System.Web.Http.ModelBinding;
using toofz.NecroDancer.Leaderboards;
using toofz.NecroDancer.Web.Api.Infrastructure;

namespace toofz.NecroDancer.Web.Api.Models
{
    [ModelBinder(BinderType = typeof(ModesBinder))]
    public sealed class Modes : LeaderboardCategoryBase
    {
        public Modes(Category category) : base(category) { }
    }
}