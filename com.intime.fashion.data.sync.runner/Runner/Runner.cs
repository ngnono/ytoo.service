using com.intime.fashion.data.sync.Wgw;
using Common.Logging;

namespace com.intime.fashion.data.sync.runner.Runner
{
    public abstract class Runner
    {
        private readonly SyncClient _client = new SyncClient();
        protected SyncClient Client { get { return _client; } }
        protected ILog Logger { get { return LogManager.GetLogger(this.GetType()); } }

        public void Run()
        {
            this.Do();
        }

        protected abstract void Do();
    }
}