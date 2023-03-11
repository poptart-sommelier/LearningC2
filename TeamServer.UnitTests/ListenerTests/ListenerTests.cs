using TeamServer.Models.Listeners;
using TeamServer.Services;

namespace TeamServer.UnitTests.ListenerTests
{
    public class ListenerTests
    {
        private readonly IListenerService _listeners;

        public ListenerTests(IListenerService listeners)
        {
            _listeners = listeners;
        }

        [Fact]
        public void TestCreateGetListener()
        {
            var origlinalListener = new HttpListener("TestListener", 4444);
            _listeners.AddListener(origlinalListener);

            var newListener = (HttpListener)_listeners.GetListener(origlinalListener.Name);

            Assert.NotNull(newListener);
            Assert.Equal(origlinalListener.Name, newListener.Name);
            Assert.Equal(origlinalListener.BindPort, newListener.BindPort);

        }
    }
}