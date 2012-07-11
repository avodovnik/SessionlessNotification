using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: WebActivator.PreApplicationStartMethod(typeof(SessionlessNotification.Web.App_Start.Notifications), "Start", Order = Int32.MaxValue)]

namespace SessionlessNotification.Web.App_Start
{
    public static class Notifications
    {
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(NotificationModule));
        }

        public class NotificationModule : IHttpModule
        {
            public class NotificationMessage
            {
                public string Message { get; set; }

                public string Type { get; set; }
            }

            public static void AddNotification(NotificationMessage message)
            {
                GetNotificationMessages().Add(message);
            }

            public static NotificationMessage PopNotificationMessage()
            {
                var messages = GetNotificationMessages();
                var message = messages.FirstOrDefault();
                if (message != null)
                    messages.Remove(message);

                return message;
            }

            private static readonly string MessageFieldName = typeof(NotificationModule).AssemblyQualifiedName + "-notification-messages";
            private const string CookieName = "app-notifications";

            private static IList<NotificationMessage> GetNotificationMessages()
            {
                var collection = HttpContext.Current.Items[MessageFieldName] as IList<NotificationMessage>;

                if (collection == null)
                {
                    collection = new List<NotificationMessage>();
                    HttpContext.Current.Items[MessageFieldName] = collection;
                }

                return collection;
            }

            public void Dispose()
            {

            }

            public void Init(HttpApplication context)
            {
                context.BeginRequest += new EventHandler(context_BeginRequest);
                context.EndRequest += new EventHandler(context_EndRequest);
            }

            void context_BeginRequest(object sender, EventArgs e)
            {
                // try and get the cookie
                var context = (HttpApplication)sender;
                var notificationCookie = context.Request.Cookies.Get(CookieName);

                if (notificationCookie == null)
                    return;

                HttpContext.Current.Items[MessageFieldName] = new JavaScriptSerializer().Deserialize<List<NotificationMessage>>(HttpUtility.UrlDecode(notificationCookie.Value));
            }

            void context_EndRequest(object sender, EventArgs e)
            {

                var context = (HttpApplication)sender;

                ForceCookieSet(context);
            }

            public static void ForceCookieSet(HttpApplication context)
            {
                var notificationCookie = context.Request.Cookies.Get(CookieName) ?? new HttpCookie(CookieName);
                notificationCookie.Value = HttpUtility.UrlEncode(new JavaScriptSerializer().Serialize(GetNotificationMessages()));

                context.Response.Cookies.Set(notificationCookie);
            }
        }
    }
}