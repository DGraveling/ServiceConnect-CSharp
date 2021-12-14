﻿//Copyright (C) 2015  Timothy Watson, Jakub Pachansky

//This program is free software; you can redistribute it and/or
//modify it under the terms of the GNU General Public License
//as published by the Free Software Foundation; either version 2
//of the License, or (at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program; if not, write to the Free Software
//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using ServiceConnect.Core;
using ServiceConnect.Interfaces;
using ServiceConnect.UnitTests.Fakes.Messages;
using Xunit;

namespace ServiceConnect.UnitTests.Stream
{
    public class CreateStreamTests
    {

        [Fact]
        public void CreateStreamShouldCreateAMessageBusWriteStream()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var mockProducer = new Mock<IProducer>();
            var mockContainer = new Mock<IBusContainer>();
            var mockRequestConfiguration = new Mock<IRequestConfiguration>();
            var mockStream = new Mock<IMessageBusWriteStream>();
            var mockSendMessagePipeline = new Mock<ISendMessagePipeline>();
            mockConfiguration.Setup(x => x.GetSendMessagePipeline()).Returns(mockSendMessagePipeline.Object);
            mockConfiguration.Setup(x => x.GetContainer()).Returns(mockContainer.Object);
            mockConfiguration.Setup(x => x.GetProducer()).Returns(mockProducer.Object);
            mockConfiguration.SetupGet(x => x.TransportSettings).Returns(new TransportSettings ());
            mockConfiguration.Setup(x => x.GetRequestConfiguration(It.IsAny<Guid>())).Returns(mockRequestConfiguration.Object);
            mockConfiguration.Setup(x=> x.GetMessageBusWriteStream(It.IsAny<IProducer>(), "TestEndpoint", It.IsAny<string>(), It.IsAny<IConfiguration>())).Returns(mockStream.Object);
            var task = new Task(() => { });
            mockRequestConfiguration.Setup(x => x.SetHandler(It.IsAny<Action<object>>())).Returns(task);

            var message = new FakeMessage1(Guid.NewGuid())
            {
                Username = "Tim Watson"
            };

            mockSendMessagePipeline.Setup(x => x.ExecuteSendMessagePipeline(It.IsAny<Type>(), It.IsAny<byte[]>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<string>())).Callback(task.Start);

            // Act
            var bus = new Bus(mockConfiguration.Object);

            // Act
            var stream = bus.CreateStream("TestEndpoint", message);

            // Assert
            Assert.NotNull(stream);
            Assert.Equal(mockStream.Object, stream);
        }

        [Fact]
        public void CreateStreamShouldSendARequestMessageToTheSpecifiedEndpoint()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var mockProducer = new Mock<IProducer>();
            var mockContainer = new Mock<IBusContainer>();
            var mockRequestConfiguration = new Mock<IRequestConfiguration>();
            var mockSendMessagePipeline = new Mock<ISendMessagePipeline>();
            mockConfiguration.Setup(x => x.GetSendMessagePipeline()).Returns(mockSendMessagePipeline.Object);
            mockConfiguration.Setup(x => x.GetContainer()).Returns(mockContainer.Object);
            mockConfiguration.Setup(x => x.GetProducer()).Returns(mockProducer.Object);
            mockConfiguration.SetupGet(x => x.TransportSettings).Returns(new TransportSettings());
            mockConfiguration.Setup(x => x.GetRequestConfiguration(It.IsAny<Guid>())).Returns(mockRequestConfiguration.Object);
            var task = new Task(() => { });
            mockRequestConfiguration.Setup(x => x.SetHandler(It.IsAny<Action<object>>())).Returns(task);

            var message = new FakeMessage1(Guid.NewGuid())
            {
                Username = "Tim Watson"
            };

            mockSendMessagePipeline.Setup(x => x.ExecuteSendMessagePipeline(It.IsAny<Type>(), It.IsAny<byte[]>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<string>())).Callback(task.Start);

            // Act
            var bus = new Bus(mockConfiguration.Object);

            // Act
            bus.CreateStream("TestEndpoint", message);

            // Assert

            mockSendMessagePipeline.Verify(x => x.ExecuteSendMessagePipeline(It.IsAny<Type>(), It.IsAny<byte[]>(), It.Is<Dictionary<string, string>>(y => y["MessageType"] == "ByteStream" && y.ContainsKey("Start")), "TestEndpoint"));
        }
    }
}