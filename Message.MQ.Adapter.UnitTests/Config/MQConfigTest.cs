using IronFramework.Common.Config;
using Megadotnet.MessageMQ.Adapter.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Message.MQ.Adapter.UnitTests.Config
{
    /// <summary>
    /// MQConfigTest
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    ///   <configSections>
    ///         <section name = "MyMQConfig" type="IronFramework.Common.Config.SectionHandler`1[[Megadotnet.MessageMQ.Adapter.Config.MyMQConfig, Megadotnet.MessageMQ.Adapter]],IronFramework.Common.Config"/>
    ///      </configSections>
    ///    <MyMQConfig>
    ///       <mqidaddress>tcp://127.0.0.1:61616/</mqidaddress>
    ///      <queuedestination>PushMessageQueue</queuedestination>
    ///    </MyMQConfig>
    /// ]]>
    /// </code>
    /// </example>
    public class MQConfigTest
    {
        [Fact]
        public void GetValue()
        { 
            Assert.NotNull(MyMQConfig.MQIpAddress);
        }
    }
}
