using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using MSS.API.Model.Data;
namespace MSS.API.Core.EventServer
{
    public interface IMssEventClient
    {
        Task RecieveMsg(MssEventMsg message);
    }
    public class MssEventHub : Hub<IMssEventClient>
    {
        private readonly EventQueues _queues;
        public MssEventHub(EventQueues queues) {
            _queues = queues;
        }
        
        public override async Task OnConnectedAsync()
        {
            // await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }
        public Task SendMessage(string message)
        {
            MssEventMsg msg = new MssEventMsg();
            msg.msg = message;
            msg.type = MssEventType.Alarm;
            msg.eqp = new Equipment(){
                ID = 1,
                EqpCode = "test",
                EqpName = "aa"
            };
            _queues.AlarmQueue.Enqueue(msg);
            return Task.CompletedTask;
            // await Clients.All.RecieveMsg(msg);
        }
    }
}