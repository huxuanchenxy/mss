using System.Collections.Concurrent;
namespace MSS.API.Core.EventServer
{
    public class EventQueues
    {
        private ConcurrentQueue<MssEventMsg>  _alarmQueue = new ConcurrentQueue<MssEventMsg>();
        private ConcurrentQueue<MssEventMsg> _warnningQueue = new ConcurrentQueue<MssEventMsg>();
        public ConcurrentQueue<MssEventMsg> AlarmQueue
        {
            get
            {
                return _alarmQueue;
            }
        }
        public ConcurrentQueue<MssEventMsg> WarnningQueue
        {
            get
            {
                return _warnningQueue;
            }
        }
    }
}