using Detector.EpisodeStorage.Main;
using Detector.EpisodeStorage.ScreenShotDB;
using Xunit;
using Xunit.Abstractions;

namespace Detector.Tests
{
    public class MessageProcessorTest
    {
        private readonly MessageProcessor _processor;
        private readonly ITestOutputHelper _testOutputHelper;

        public MessageProcessorTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _processor = new MessageProcessor(new FileStorage(null, null), new RSAProvider(null, null));
        }
        [Fact]
        public void ProcessMessage()
        {
            //_processor.ProcessMessage(1, "{ \"Command\": \"store_file\", \"FileName\": \"buffer.test.log\", \"Data\": \"dGltZ\"}").
            //    ContinueWith(res =>
            //    {
            //        _testOutputHelper.WriteLine(res.Result);
            //    });

        }
    }
}
