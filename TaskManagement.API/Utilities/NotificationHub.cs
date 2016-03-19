using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using TaskManagement.API.Utilities;

namespace TaskManagement.API
{
    public class NotificationHub : Hub
    {
        private readonly static ConnectionMapping _connections = new ConnectionMapping();

        public void Subscribe(string connectionId, string username)
        {
            _connections.Add(username, connectionId);
        }

        public void Unsubscribe(string username)
        {
            var connectionIds = _connections.GetConnections(username);
            connectionIds.ToList().ForEach(c => _connections.Remove(username, c));
        }

        public static void NotifyAccept(string usernameSender, string username)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            var connectionsIds = _connections.GetConnections(usernameSender).ToList();
            connectionsIds.ForEach(c => hubContext.Clients.Client(c).notifyAccept(username + " accepted your friend request."));
        }

        public static void NotifyNewFriendRequest(string usernameTo)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            var connectionsIds = _connections.GetConnections(usernameTo).ToList();
            connectionsIds.ForEach(c => hubContext.Clients.Client(c).notifyNewFriendRequest());
        }

        public static void NotifyFriendRequestRejected(string usernameTo)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            var connectionsIds = _connections.GetConnections(usernameTo).ToList();
            connectionsIds.ForEach(c => hubContext.Clients.Client(c).notifyFriendRequestRejected());
        }

        public static void NotifyFriendshipDeleted(string usernameTo)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            var connectionsIds = _connections.GetConnections(usernameTo).ToList();
            connectionsIds.ForEach(c => hubContext.Clients.Client(c).notifyFriendshipDeleted());
        }

        public static void NotifyRefreshTask(List<string> usernames, int taskId)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            usernames.ForEach(u =>
            {
                var connectionIds = _connections.GetConnections(u).ToList();
                connectionIds.ForEach(c => hubContext.Clients.Client(c).notifyRefreshTask(taskId));
            });
        }

        public static void NotifyRefreshTasks(List<string> usernames, int taskId)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            usernames.ForEach(u =>
            {
                var connectionIds = _connections.GetConnections(u).ToList();
                connectionIds.ForEach(c => hubContext.Clients.Client(c).notifyRefreshTasks(taskId));
            });
        }
    }
}