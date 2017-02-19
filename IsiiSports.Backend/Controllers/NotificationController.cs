using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using IsiiSports.Backend.Helpers;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Newtonsoft.Json;

namespace IsiiSports.Backend.Controllers
{
    public class NotificationController : ApiController
    {        
        private readonly NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(ConfigurationManager.AppSettings["NotificationHubConnection"], ConfigurationManager.AppSettings["HubName"]);

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
        }

        #region Notification

        public async Task NotifyByTags(string message, List<string> tags, NotificationPayload payload = null, int? badgeCount = null)
        {
            if (Startup.IsDemoMode)
                return;

            var notification = new Dictionary<string, string> { { "message", message } };

            if (payload != null)
            {
                var json = JsonConvert.SerializeObject(payload);
                notification.Add("payload", json);
            }
            else
            {
                notification.Add("payload", "");
            }

            if (badgeCount == null)
                badgeCount = 0;

            notification.Add("badge", badgeCount.Value.ToString());

            try
            {
                await hub.SendTemplateNotificationAsync(notification, tags);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task NotifyByTag(string message, string tag, NotificationPayload payload = null, int? badgeCount = null)
        {
            await NotifyByTags(message, new List<string> { tag }, payload, badgeCount);
        }

        #endregion

        #region Registration

        [HttpPut]
        [Route("api/registerWithHub")]
        public async Task<string> RegisterWithHub(DeviceRegistration deviceUpdate)
        {
            string newRegistrationId = null;
            try
            {
                // make sure there are no existing registrations for this push handle (used for iOS and Android)
                if (deviceUpdate.Handle != null)
                {
                    //Azure likes to uppercase the iOS device handles for some reason - no worries tho, I only spent 2 hours tracking this down
                    if (deviceUpdate.Platform == "iOS")
                        deviceUpdate.Handle = deviceUpdate.Handle.ToUpper();

                    var registrations = await hub.GetRegistrationsByChannelAsync(deviceUpdate.Handle, 100);

                    foreach (var reg in registrations)
                    {
                        if (newRegistrationId == null)
                        {
                            newRegistrationId = reg.RegistrationId;
                        }
                        else
                        {
                            await hub.DeleteRegistrationAsync(reg);
                        }
                    }
                }

                if (newRegistrationId == null)
                    newRegistrationId = await hub.CreateRegistrationIdAsync();

                RegistrationDescription registration = null;

                switch (deviceUpdate.Platform)
                {
                    case "iOS":
                        const string alertTemplate = "{\"aps\":{\"alert\":\"$(message)\",\"badge\":\"#(badge)\",\"payload\":\"$(payload)\"}}";
                        registration = new AppleTemplateRegistrationDescription(deviceUpdate.Handle, alertTemplate);
                        break;
                    case "Android":
                        const string messageTemplate = "{\"data\":{\"title\":\"Sport\",\"message\":\"$(message)\",\"payload\":\"$(payload)\"}}";
                        registration = new GcmTemplateRegistrationDescription(deviceUpdate.Handle, messageTemplate);
                        break;
                    default:
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                registration.RegistrationId = newRegistrationId;
                registration.Tags = new HashSet<string>(deviceUpdate.Tags);
                await hub.CreateOrUpdateRegistrationAsync(registration);
            }
            catch (Exception ex)
            {
                throw ex.GetBaseException().Message.ToException(Request);
            }

            return newRegistrationId;
        }

        [HttpGet]
        [Route("api/sendTestPushNotification")]
        public async Task SendTestPushNotification(string athleteId)
        {
            if (Startup.IsDemoMode)
            {
                throw "Push notifications are disabled in DEMO mode".ToException(Request);
            }

            if (athleteId != null)
            {
                var message = "Push notifications are working for you - excellent!";
                await NotifyByTag(message, athleteId);
            }
        }

        [HttpDelete]
        [Route("api/unregister")]
        public async Task<HttpResponseMessage> Delete(string id)
        {
            await hub.DeleteRegistrationAsync(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private static void ReturnGoneIfHubResponseIsGone(MessagingException e)
        {
            var webex = e.InnerException as WebException;
            if (webex == null || webex.Status != WebExceptionStatus.ProtocolError)
                return;

            var response = (HttpWebResponse)webex.Response;
            if (response.StatusCode == HttpStatusCode.Gone)
                throw new HttpRequestException(HttpStatusCode.Gone.ToString());
        }

        #endregion
    }
}