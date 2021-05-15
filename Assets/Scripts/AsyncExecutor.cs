using System.Threading;

namespace Innerclash {
    public class AsyncExecutor {
        public void H(ThreadStart h) {
            new Thread(h).Start();
        }
    }
}
