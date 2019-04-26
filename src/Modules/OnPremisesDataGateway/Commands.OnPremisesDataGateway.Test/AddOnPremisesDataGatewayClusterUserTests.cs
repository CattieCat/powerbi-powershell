﻿/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

using System;
using System.Management.Automation;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.PowerBI.Commands.Common.Test;
using Microsoft.PowerBI.Commands.Profile.Test;
using Microsoft.PowerBI.Common.Api;
using Microsoft.PowerBI.Common.Api.Gateways.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.PowerBI.Commands.OnPremisesDataGateway.Test
{
    [TestClass]
    public class AddOnPremisesDataGatewayClusterUserTests
    {
        private static CmdletInfo AddOnPremisesDataGatewayClusterUserInfo { get; } = new CmdletInfo(
            $"{AddOnPremisesDataGatewayClusterUser.CmdletVerb}-{AddOnPremisesDataGatewayClusterUser.CmdletName}",
            typeof(AddOnPremisesDataGatewayClusterUser));

        [TestMethod]
        [TestCategory("Interactive")]
        [TestCategory("SkipWhenLiveUnitTesting")] // Ignore for Live Unit Testing
        public void EndToEndAddOnPremisesDataGatewayClusterUser()
        {
            using (var ps = System.Management.Automation.PowerShell.Create())
            {
                // Arrange
                ProfileTestUtilities.ConnectToPowerBI(ps);
                ps.AddCommand(AddOnPremisesDataGatewayClusterUserInfo)
                    .AddParameter(nameof(AddOnPremisesDataGatewayClusterUser.GatewayClusterId), new Guid())
                    .AddParameter(nameof(AddOnPremisesDataGatewayClusterUser.PrincipalObjectId), new Guid("{F31FEA72-8435-4871-BF75-E94168C71A6D}"))
                    .AddParameter(nameof(AddOnPremisesDataGatewayClusterUser.AllowedDataSourceTypes), new DatasourceType[] { DatasourceType.Sql })
                    .AddParameter(nameof(AddOnPremisesDataGatewayClusterUser.Role), "ConnectionCreator");

                // Act
                var result = ps.Invoke();

                // Assert
                TestUtilities.AssertNoCmdletErrors(ps);
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void AddOnPremisesDataGatewayClusterUserReturnsExpectedResults()
        {
            // Arrange
            var gatewayClusterId = Guid.NewGuid();
            var request = new GatewayClusterAddPrincipalRequest()
            {
                PrincipalObjectId = Guid.NewGuid(),
                AllowedDataSourceTypes = new DatasourceType[]
                {
                                DatasourceType.Sql
                },
                Role = "the role"

            };

            var expectedResponse = new HttpResponseMessage();
            var client = new Mock<IPowerBIApiClient>();
            client.Setup(x => x.Gateways
                .AddUsersToGatewayCluster(It.IsAny<Guid>(), It.IsAny<GatewayClusterAddPrincipalRequest>(), true))
                .ReturnsAsync(expectedResponse);

            var initFactory = new TestPowerBICmdletInitFactory(client.Object);
            var cmdlet = new AddOnPremisesDataGatewayClusterUser(initFactory)
            {
                GatewayClusterId = Guid.NewGuid(),
                PrincipalObjectId = request.PrincipalObjectId,
                AllowedDataSourceTypes = request.AllowedDataSourceTypes,
                Role = request.Role
            };

            // Act
            cmdlet.InvokePowerBICmdlet();

            // Assert
            TestUtilities.AssertExpectedUnitTestResults(expectedResponse, client, initFactory);
        }
    }
}
