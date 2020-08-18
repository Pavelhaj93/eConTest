using eContracting.Kernel.Models;
using eContracting.Kernel.Services;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace eContracting.Kernel.Tests.Services
{
    [TestFixture]
    public class LoginsCheckerClientTests
    {
        [TestCase(ExpectedResult = false)]
        public bool CanLogin_No_Due_To_Max_Failed_Attempts()
        {
            string guid = Guid.NewGuid().ToString("N");
            string sessionId = "SessionId=" + Guid.NewGuid().ToString("N");
            string browserInfo = "NUnit test adapter";

            FilterDefinitionBuilder<FailedLoginInfoModel> builder = Builders<FailedLoginInfoModel>.Filter;
            FilterDefinition<FailedLoginInfoModel> filter = builder.Eq(x => x.Guid, guid);

            Mock<IAsyncCursor<FailedLoginInfoModel>> iAsyncCursor = new Mock<IAsyncCursor<FailedLoginInfoModel>>();
            iAsyncCursor.Setup(x => x.MoveNext(default(CancellationToken))).Returns(false);

            Mock<IMongoCollection<FailedLoginInfoModel>> collection = new Mock<IMongoCollection<FailedLoginInfoModel>>();
            collection.Setup(x => x.FindSync(filter, (FindOptions<FailedLoginInfoModel, FailedLoginInfoModel>)null, CancellationToken.None)).Returns(iAsyncCursor.Object);

            LoginsCheckerClient client = new LoginsCheckerClient(3, new TimeSpan(0, 10, 0), collection.Object);
            client.AddFailedAttempt(guid, sessionId, browserInfo);

            Thread.Sleep(new TimeSpan(0, 0, 1));

            client.AddFailedAttempt(guid, sessionId, browserInfo);

            Thread.Sleep(new TimeSpan(0, 0, 1));

            client.AddFailedAttempt(guid, sessionId, browserInfo);

            Thread.Sleep(new TimeSpan(0, 0, 1));

            return client.CanLogin(guid);
        }

        /// <summary>
        /// This is live test with direct connection to MongoDB (just to verify real behavior on your local computer)
        /// </summary>
        /// <returns></returns>
        //[TestCase(ExpectedResult = false)]
        public bool CanLogin_No_Due_To_Max_Failed_Attempts__Live_Test()
        {
            string guid = Guid.NewGuid().ToString("N");
            string sessionId = "SessionId=" + Guid.NewGuid().ToString("N");
            string browserInfo = "NUnit test adapter";

            LoginsCheckerClient client = new LoginsCheckerClient(3, new TimeSpan(0, 10, 0));
            client.AddFailedAttempt(guid, sessionId, browserInfo);

            Thread.Sleep(new TimeSpan(0, 0, 1));

            client.AddFailedAttempt(guid, sessionId, browserInfo);

            Thread.Sleep(new TimeSpan(0, 0, 1));

            client.AddFailedAttempt(guid, sessionId, browserInfo);

            Thread.Sleep(new TimeSpan(0, 0, 1));

            return client.CanLogin(guid);
        }

        /// <summary>
        /// This is live test with direct connection to MongoDB (just to verify real behavior on your local computer)
        /// </summary>
        //[TestCase(ExpectedResult = true)]
        public bool CanLogin_Yes_Due_To_Delay__Live_Test()
        {
            string guid = Guid.NewGuid().ToString("N");
            string sessionId = "SessionId=" + Guid.NewGuid().ToString("N");
            string browserInfo = "NUnit test adapter";

            LoginsCheckerClient client = new LoginsCheckerClient(3, new TimeSpan(0, 0, 1));
            client.AddFailedAttempt(guid, sessionId, browserInfo);

            Thread.Sleep(new TimeSpan(0, 0, 1));

            client.AddFailedAttempt(guid, sessionId, browserInfo);

            Thread.Sleep(new TimeSpan(0, 0, 1));

            client.AddFailedAttempt(guid, sessionId, browserInfo);

            Thread.Sleep(new TimeSpan(0, 0, 3));

            return client.CanLogin(guid);
        }
    }
}
