﻿using System;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata.Providers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using toofz.NecroDancer.Leaderboards;
using toofz.NecroDancer.Web.Api.Infrastructure;
using toofz.NecroDancer.Web.Api.Models;

namespace toofz.NecroDancer.Web.Api.Tests.Infrastructure
{
    class CharactersBinderTests
    {
        [TestClass]
        public class Constructor
        {
            [TestMethod]
            public void LeaderboardCategoriesIsNull_ThrowsArgumentNullException()
            {
                // Arrange
                Categories leaderboardCategories = null;

                // Act -> Assert
                Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    new CharactersBinder(leaderboardCategories);
                });
            }

            [TestMethod]
            public void ReturnsInstance()
            {
                // Arrange
                var leaderboardCategories = LeaderboardsResources.ReadLeaderboardCategories();

                // Act
                var binder = new CharactersBinder(leaderboardCategories);

                // Assert
                Assert.IsInstanceOfType(binder, typeof(CharactersBinder));
            }
        }

        [TestClass]
        public class GetModelMethod
        {
            [TestMethod]
            public void ReturnsModel()
            {
                // Arrange
                var leaderboardCategories = LeaderboardsResources.ReadLeaderboardCategories();
                var binder = new CharactersBinder(leaderboardCategories);
                HttpActionContext actionContext = null;
                var modelName = "myModelName";
                var mockValueProvider = new Mock<IValueProvider>();
                mockValueProvider.Setup(v => v.GetValue("myModelName")).Returns(Util.CreateValueProviderResult("cadence"));
                var valueProvider = mockValueProvider.Object;
                var data = new DataAnnotationsModelMetadataProvider();
                var modelMetadata = data.GetMetadataForType(null, typeof(Characters));
                var bindingContext = new ModelBindingContext
                {
                    ModelName = modelName,
                    ValueProvider = valueProvider,
                    ModelMetadata = modelMetadata,
                };

                // Act
                binder.BindModel(actionContext, bindingContext);

                // Assert
                var model = bindingContext.Model;
                Assert.IsInstanceOfType(model, typeof(Characters));
            }
        }
    }
}