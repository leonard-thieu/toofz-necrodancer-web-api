﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.OAuth.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using toofz.NecroDancer.Web.Api.Identity;
using toofz.NecroDancer.Web.Api.Providers;

namespace toofz.NecroDancer.Web.Api.Tests.Providers
{
    public class ApplicationOAuthProviderTests
    {
        [TestClass]
        public class Constructor
        {
            [TestMethod]
            public void PublicClientIdIsNull_ThrowsArgumentNullException()
            {
                // Arrange
                string publicClientId = null;

                // Act -> Assert
                Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    new ApplicationOAuthProvider(publicClientId);
                });
            }

            [TestMethod]
            public void ReturnsInstance()
            {
                // Arrange
                var publicClientId = "myPublicClientId";

                // Act
                var provider = new ApplicationOAuthProvider(publicClientId);

                // Assert
                Assert.IsInstanceOfType(provider, typeof(ApplicationOAuthProvider));
            }
        }

        [TestClass]
        public class GrantResourceOwnerCredentialsMethod
        {
            [TestMethod]
            public async Task UserNotFound_SetsError()
            {
                // Arrange
                var publicClientId = "myPublicClientId";
                var provider = new ApplicationOAuthProvider(publicClientId);
                var userManager = Mock.Of<ApplicationUserManagerAdapter>();
                var mockOwinContext = new Mock<IOwinContext>();
                mockOwinContext.Setup(c => c.Get<ApplicationUserManager>(It.IsAny<string>())).Returns(userManager);
                var owinContext = mockOwinContext.Object;
                var options = new OAuthAuthorizationServerOptions();
                var clientId = "myPublicClientId";
                var userName = "myUserName";
                var password = "myPassword";
                var scope = new List<string>();
                var context = new OAuthGrantResourceOwnerCredentialsContext(owinContext, options, clientId, userName, password, scope);

                // Act
                await provider.GrantResourceOwnerCredentials(context);

                // Assert
                Assert.AreEqual("invalid_grant", context.Error);
            }

            [TestMethod]
            public async Task Validates()
            {
                // Arrange
                var publicClientId = "myPublicClientId";
                var provider = new ApplicationOAuthProvider(publicClientId);
                var userName = "myUserName";
                var password = "myPassword";
                var mockUserManager = new Mock<ApplicationUserManagerAdapter>();
                mockUserManager.Setup(m => m.FindAsync(userName, password)).Returns(Task.FromResult(new ApplicationUser()));
                var userManager = mockUserManager.Object;
                var mockOwinRequest = new Mock<IOwinRequest>();
                var owinRequest = mockOwinRequest.Object;
                var mockAuthenticationManager = new Mock<IAuthenticationManager>();
                var authenticationManager = mockAuthenticationManager.Object;
                var mockOwinContext = new Mock<IOwinContext>();
                mockOwinContext.Setup(c => c.Get<ApplicationUserManager>(It.IsAny<string>())).Returns(userManager);
                mockOwinContext.SetupGet(c => c.Request).Returns(owinRequest);
                mockOwinContext.SetupGet(c => c.Authentication).Returns(authenticationManager);
                var owinContext = mockOwinContext.Object;
                mockOwinRequest.SetupGet(r => r.Context).Returns(owinContext);
                var options = new OAuthAuthorizationServerOptions();
                var clientId = "myPublicClientId";
                var scope = new List<string>();
                var context = new OAuthGrantResourceOwnerCredentialsContext(owinContext, options, clientId, userName, password, scope);

                // Act
                await provider.GrantResourceOwnerCredentials(context);

                // Assert
                Assert.IsTrue(context.IsValidated);
                Assert.IsFalse(context.HasError);
            }

            [TestMethod]
            public async Task SignsIn()
            {
                // Arrange
                var publicClientId = "myPublicClientId";
                var provider = new ApplicationOAuthProvider(publicClientId);
                var userName = "myUserName";
                var password = "myPassword";
                var mockUserManager = new Mock<ApplicationUserManagerAdapter>();
                mockUserManager.Setup(m => m.FindAsync(userName, password)).Returns(Task.FromResult(new ApplicationUser()));
                var userManager = mockUserManager.Object;
                var mockOwinRequest = new Mock<IOwinRequest>();
                var owinRequest = mockOwinRequest.Object;
                var mockAuthenticationManager = new Mock<IAuthenticationManager>();
                var authenticationManager = mockAuthenticationManager.Object;
                var mockOwinContext = new Mock<IOwinContext>();
                mockOwinContext.Setup(c => c.Get<ApplicationUserManager>(It.IsAny<string>())).Returns(userManager);
                mockOwinContext.SetupGet(c => c.Request).Returns(owinRequest);
                mockOwinContext.SetupGet(c => c.Authentication).Returns(authenticationManager);
                var owinContext = mockOwinContext.Object;
                mockOwinRequest.SetupGet(r => r.Context).Returns(owinContext);
                var options = new OAuthAuthorizationServerOptions();
                var clientId = "myPublicClientId";
                var scope = new List<string>();
                var context = new OAuthGrantResourceOwnerCredentialsContext(owinContext, options, clientId, userName, password, scope);

                // Act
                await provider.GrantResourceOwnerCredentials(context);

                // Assert
                mockAuthenticationManager.Verify(a => a.SignIn(It.IsAny<ClaimsIdentity>()), Times.Once);
            }

            public class ApplicationUserManagerAdapter : ApplicationUserManager
            {
                public ApplicationUserManagerAdapter() : base(Mock.Of<IUserStore<ApplicationUser>>()) { }
            }
        }

        [TestClass]
        public class TokenEndpointMethod
        {
            [TestMethod]
            public async Task AddsAdditionalResponseParameters()
            {
                // Arrange
                var publicClientId = "myPublicClientId";
                var provider = new ApplicationOAuthProvider(publicClientId);
                var owinContext = Mock.Of<IOwinContext>();
                var options = new OAuthAuthorizationServerOptions();
                var identity = new ClaimsIdentity();
                var properties = new AuthenticationProperties();
                properties.Dictionary.Add("myKey", "myValue");
                var ticket = new AuthenticationTicket(identity, properties);
                var parameters = Mock.Of<IReadableStringCollection>();
                var tokenEndpointRequest = new TokenEndpointRequest(parameters);
                var context = new OAuthTokenEndpointContext(owinContext, options, ticket, tokenEndpointRequest);

                // Act
                await provider.TokenEndpoint(context);

                // Assert
                Assert.IsTrue(context.AdditionalResponseParameters.ContainsKey("myKey"));
                var value = context.AdditionalResponseParameters["myKey"];
                Assert.AreEqual("myValue", value);
            }
        }

        [TestClass]
        public class ValidateClientAuthenticationMethod
        {
            [TestMethod]
            public async Task ClientIdIsNull_Validates()
            {
                // Arrange
                var publicClientId = "myPublicClientId";
                var provider = new ApplicationOAuthProvider(publicClientId);
                var owinContext = Mock.Of<IOwinContext>();
                var options = new OAuthAuthorizationServerOptions();
                var parameters = Mock.Of<IReadableStringCollection>();
                var context = new OAuthValidateClientAuthenticationContext(owinContext, options, parameters);

                // Act
                await provider.ValidateClientAuthentication(context);

                // Assert
                Assert.IsTrue(context.IsValidated);
                Assert.IsFalse(context.HasError);
            }

            [TestMethod]
            public async Task ClientIdIsNotNull_DoesNotValidate()
            {
                // Arrange
                var publicClientId = "myPublicClientId";
                var provider = new ApplicationOAuthProvider(publicClientId);
                var owinContext = Mock.Of<IOwinContext>();
                var options = new OAuthAuthorizationServerOptions();
                var parameters = Mock.Of<IReadableStringCollection>();
                var clientId = "myClientId";
                var context = new OAuthValidateClientAuthenticationContextAdapter(owinContext, options, parameters, clientId);

                // Act
                await provider.ValidateClientAuthentication(context);

                // Assert
                Assert.IsFalse(context.IsValidated);
            }

            sealed class OAuthValidateClientAuthenticationContextAdapter : OAuthValidateClientAuthenticationContext
            {
                public OAuthValidateClientAuthenticationContextAdapter(IOwinContext context, OAuthAuthorizationServerOptions options, IReadableStringCollection parameters, string clientId) :
                    base(context, options, parameters)
                {
                    ClientId = clientId;
                }
            }
        }

        [TestClass]
        public class ValidateClientRedirectUriMethod
        {
            [TestMethod]
            public async Task ClientIdAndRedirectUriMatch_Validates()
            {
                // Arrange
                var publicClientId = "myPublicClientId";
                var provider = new ApplicationOAuthProvider(publicClientId);
                var mockOwinRequest = new Mock<IOwinRequest>();
                mockOwinRequest.SetupGet(r => r.Uri).Returns(new Uri("http://example.org/"));
                var owinRequest = mockOwinRequest.Object;
                var mockOwinContext = new Mock<IOwinContext>();
                mockOwinContext.SetupGet(c => c.Request).Returns(owinRequest);
                var owinContext = mockOwinContext.Object;
                var options = new OAuthAuthorizationServerOptions();
                var clientId = "myPublicClientId";
                var redirectUri = "http://example.org/";
                var context = new OAuthValidateClientRedirectUriContext(owinContext, options, clientId, redirectUri);

                // Act
                await provider.ValidateClientRedirectUri(context);

                // Assert
                Assert.IsTrue(context.IsValidated);
                Assert.IsFalse(context.HasError);
            }

            [TestMethod]
            public async Task ClientIdMatchesAndRedirectUriDoesNotMatch_DoesNotValidate()
            {
                // Arrange
                var publicClientId = "myPublicClientId";
                var provider = new ApplicationOAuthProvider(publicClientId);
                var mockOwinRequest = new Mock<IOwinRequest>();
                mockOwinRequest.SetupGet(r => r.Uri).Returns(new Uri("http://example.com/"));
                var owinRequest = mockOwinRequest.Object;
                var mockOwinContext = new Mock<IOwinContext>();
                mockOwinContext.SetupGet(c => c.Request).Returns(owinRequest);
                var owinContext = mockOwinContext.Object;
                var options = new OAuthAuthorizationServerOptions();
                var clientId = "myPublicClientId";
                var redirectUri = "http://example.org/";
                var context = new OAuthValidateClientRedirectUriContext(owinContext, options, clientId, redirectUri);

                // Act
                await provider.ValidateClientRedirectUri(context);

                // Assert
                Assert.IsFalse(context.IsValidated);
            }

            [TestMethod]
            public async Task ClientIdDoesNotMatch_DoesNotValidate()
            {
                // Arrange
                var publicClientId = "myPublicClientId";
                var provider = new ApplicationOAuthProvider(publicClientId);
                var mockOwinRequest = new Mock<IOwinRequest>();
                mockOwinRequest.SetupGet(r => r.Uri).Returns(new Uri("http://example.com/"));
                var owinRequest = mockOwinRequest.Object;
                var mockOwinContext = new Mock<IOwinContext>();
                mockOwinContext.SetupGet(c => c.Request).Returns(owinRequest);
                var owinContext = mockOwinContext.Object;
                var options = new OAuthAuthorizationServerOptions();
                var clientId = "myInvalidPublicClientId";
                var redirectUri = "http://example.org/";
                var context = new OAuthValidateClientRedirectUriContext(owinContext, options, clientId, redirectUri);

                // Act
                await provider.ValidateClientRedirectUri(context);

                // Assert
                Assert.IsFalse(context.IsValidated);
            }
        }

        [TestClass]
        public class CreatePropertiesMethod
        {
            [TestMethod]
            public void ReturnsAuthenticationProperties()
            {
                // Arrange
                var userName = "myUserName";

                // Act
                var properties = ApplicationOAuthProvider.CreateProperties(userName);

                // Assert
                Assert.IsTrue(properties.Dictionary.ContainsKey("userName"));
                Assert.AreEqual(userName, properties.Dictionary["userName"]);
            }
        }
    }
}