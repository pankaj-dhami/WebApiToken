using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Http;
using WebApiTokenAuth.Hubs;
using WebApiTokenAuth.Model;
using WebApiTokenAuth.Utility;

namespace WebApiTokenAuth.Controller
{
    public class NotificationsController : ApiController
    {
        delegate void Delegate_SendNotification(string pns, string message, string to_tag, string userName);
        public async Task<HttpResponseMessage> Post(string pns, [FromBody]string message, string to_tag, string userName)
        {
            var user = userName;
            string[] userTag = new string[2];
            userTag[0] = "username:" + to_tag;
            userTag[1] = "from:" + user;

            Microsoft.ServiceBus.Notifications.NotificationOutcome outcome = null;
            HttpStatusCode ret = HttpStatusCode.InternalServerError;

            switch (pns.ToLower())
            {
                case "wns":
                    // Windows 8.1 / Windows Phone 8.1
                    var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
                                "From " + user + ": " + message + "</text></binding></visual></toast>";
                    outcome = await Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast, userTag);
                    break;
                case "apns":
                    // iOS
                    var alert = "{\"aps\":{\"alert\":\"" + "From " + user + ": " + message + "\"}}";
                    outcome = await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, userTag);
                    break;
                case "gcm":
                    // Android
                    var notif = "{ \"data\" : {\"message\":\"" + "From " + user + ": " + message + "\"}}";
                    outcome = await Notifications.Instance.Hub.SendGcmNativeNotificationAsync(notif, userTag);
                    break;
            }

            if (outcome != null)
            {
                if (!((outcome.State == Microsoft.ServiceBus.Notifications.NotificationOutcomeState.Abandoned) ||
                    (outcome.State == Microsoft.ServiceBus.Notifications.NotificationOutcomeState.Unknown)))
                {
                    ret = HttpStatusCode.OK;
                }
            }

            return Request.CreateResponse(ret);
        }

        [AllowAnonymous]
        [ActionName("GetPendingMsg")]
        public IHttpActionResult GetPendingMsg(int userID)
        {
            using (var scope = new TransactionScope())
            {
                var pendingMessage = (from item in MessengerHub.pendingMessageList
                                      where item.UserID == userID
                                      select new MessageModel
                                      {
                                          UserModel = item.UserModel,
                                          TextMessage = item.TextMessage
                                      }).ToList();

                MessengerHub.pendingMessageList.RemoveAll(a => a.UserID == userID);
                scope.Complete();
                return Ok(pendingMessage);
            }
        }

        [AllowAnonymous]
        [ActionName("SendMessage"), HttpPost]
        public IHttpActionResult SendMessage([FromBody] SendMsgModel sendMsgModel)
        {
            try
            {

                UserMsg fromUser = sendMsgModel.FromUser;
                MessageModel message = sendMsgModel.Message;

                int toUser = message.UserModel.UserID;
                string toUserMobile = message.UserModel.MobileNo;
                message.UserModel = fromUser;
                message.UserID = toUser;

                if (message.IsAttchment)
                {
                    if (message.Pic64Data != null && message.Pic64Data.Count() > 0)
                    {
                        string b64PicData = string.Join("/", message.Pic64Data);
                        if (!string.IsNullOrEmpty(b64PicData))
                        {
                            message.AttachmentUrl = BlobUploadUtility.UploadFileStreamToBlob(BlobUploadUtility.Base64Decode(b64PicData), fromUser.UserID + fromUser.MobileNo + DateTime.Now, BlobUploadUtility.CONTAINER_CHATFILES);
                        }
                    }
                }
                using (var scope = new TransactionScope())
                {
                    MessengerHub.pendingMessageList.Add(message);
                    int pendingMsgcount = MessengerHub.pendingMessageList.Where(a => a.UserID == toUser).Count();
                    scope.Complete();

                    if (pendingMsgcount <= 2)
                    {
                        Delegate_SendNotification async = new Delegate_SendNotification(SendNotification);
                        async.BeginInvoke("gcm", "pendingMessage", toUserMobile, "pankaj", null, null);
                    }
                }

                return Ok();

            }
            catch (Exception)
            {
                return BadRequest();
            }
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
