﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using toofz.NecroDancer.Leaderboards;
using toofz.NecroDancer.Leaderboards.EntityFramework;
using toofz.NecroDancer.Web.Api.Controllers;
using toofz.NecroDancer.Web.Api.Models;
using toofz.TestsShared;

namespace toofz.NecroDancer.Web.Api.Tests.Controllers
{
    public class ReplaysControllerTests
    {
        [TestClass]
        public class GetReplays
        {
            [TestMethod]
            public async Task ReturnsOk()
            {
                // Arrange
                var mockSet = MockHelper.MockSet<Leaderboards.Replay>();

                var mockRepository = new Mock<LeaderboardsContext>();
                mockRepository
                    .Setup(x => x.Replays)
                    .Returns(mockSet.Object);

                var mockILeaderboardsStoreClient = new Mock<ILeaderboardsStoreClient>();

                var controller = new ReplaysController(mockRepository.Object, mockILeaderboardsStoreClient.Object);

                // Act
                var actionResult = await controller.GetReplays(new ReplaysPagination());
                var contentResult = actionResult as OkNegotiatedContentResult<Api.Models.Replays>;

                // Assert
                Assert.IsNotNull(contentResult);
                Assert.IsNotNull(contentResult.Content);
            }
        }

        [TestClass]
        public class PostReplays
        {
            [TestMethod]
            public async Task ReturnsOk()
            {
                // Arrange
                var mockRepository = new Mock<LeaderboardsContext>();

                var mockILeaderboardsStoreClient = new Mock<ILeaderboardsStoreClient>();

                var controller = new ReplaysController(mockRepository.Object, mockILeaderboardsStoreClient.Object);

                // Act
                var actionResult = await controller.PostReplays(new List<ReplayModel>());
                var contentResult = actionResult as OkNegotiatedContentResult<BulkStore>;

                // Assert
                Assert.IsNotNull(contentResult);
                Assert.IsNotNull(contentResult.Content);
            }
        }
    }
}