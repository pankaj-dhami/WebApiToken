using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using WebApiTokenAuth.Model;
using System.Net.Http;
using System.Net;

namespace WebApiTokenAuth.Hubs
{
    public class MessengerHub : Hub
    {
        public static List<UserMsg> activeUserList = new List<UserMsg>();
        public static List<MessageModel> pendingMessageList = new List<MessageModel>();
        delegate void Delegate_SendNotification(string pns, string message, string to_tag, string userName);
        public void Hello(string msg)
        {
            Clients.All.hello(msg);
        }

        public void sendMessage(UserMsg fromUser, MessageModel message)
        {
            int toUser = message.UserModel.UserID;
            string toUserMobile = message.UserModel.MobileNo;
            message.UserModel = fromUser;
            message.UserID = toUser;
            //if (activeUserList.Exists(a=>a.UserID==message.UserModel.UserID))
            //{
            //    message.UserModel = fromUser;
            //    Clients.Group(toUser).getMessages(message);
            //}
            //else
            //{

            pendingMessageList.Add(message);
            Delegate_SendNotification async = new Delegate_SendNotification(SendNotification);
            async.BeginInvoke("gcm", "pendingMessage", toUserMobile, "pankaj", null, null);


            //}


        }

        public void connectUser(UserMsg user)
        {
            Groups.Add(Context.ConnectionId, user.UserID.ToString());
            user.ConnectionID = Context.ConnectionId;
            activeUserList.Add(user);
            Clients.Caller.onConnected(user);

        }

        public void getAllUserList()
        {
            foreach (var item in activeUserList)
            {
                Clients.Group(item.UserID.ToString()).refreshUserList(activeUserList);
            }
        }
        public void disconnectUser(UserMsg userModel)
        {
            Groups.Remove(Context.ConnectionId, userModel.UserID.ToString());
            activeUserList.RemoveAll(a => a.UserID == userModel.UserID);

        }
        public override Task OnDisconnected(bool stopCalled)
        {
            var user = activeUserList.Where(a => a.ConnectionID == Context.ConnectionId).FirstOrDefault();
            if (user != null)
            {
                Groups.Remove(Context.ConnectionId, user.UserID.ToString());
                activeUserList.RemoveAll(a => a.UserID == user.UserID);
            }
            //custom logic here
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        public void SendNotification(string pns, string message, string to_tag, string userName)
        {
            var user = userName;
            string[] userTag = new string[2];
            userTag[0] = "username:" + to_tag;
            userTag[1] = "from:" + user;

            Task<Microsoft.ServiceBus.Notifications.NotificationOutcome> outcome = null;
            HttpStatusCode ret = HttpStatusCode.InternalServerError;

            switch (pns.ToLower())
            {
                case "wns":
                    // Windows 8.1 / Windows Phone 8.1
                    var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
                                "From " + user + ": " + message + "</text></binding></visual></toast>";
                    outcome = Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast, userTag);
                    break;
                case "apns":
                    // iOS
                    var alert = "{\"aps\":{\"alert\":\"" + "From " + user + ": " + message + "\"}}";
                    outcome = Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, userTag);
                    break;
                case "gcm":
                    // Android
                    var notif = "{ \"data\" : {\"message\":\"" + message + "\"}}";
                    outcome = Notifications.Instance.Hub.SendGcmNativeNotificationAsync(notif, userTag);
                    break;
            }

            if (outcome != null)
            {

            }

        }
    }
}