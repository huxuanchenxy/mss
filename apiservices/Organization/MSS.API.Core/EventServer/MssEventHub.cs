using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using MSS.API.Model.Data;
using MSS.API.Core.Common;
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
            if (Context.Items["isSuper"] != null && (bool)Context.Items["isSuper"])
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, RedisKeyPrefix.SuperGroup);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.Items["isSuper"] != null && (bool)Context.Items["isSuper"])
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, RedisKeyPrefix.SuperGroup);
            }
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