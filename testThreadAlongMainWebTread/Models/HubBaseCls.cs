using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace testThreadAlongMainWebTread.Models
{
    public class ChatHub : Hub
    {
        public void SendAll(DateTime dt )
        {

            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
            string sep = "/";
            var ttp = pc.GetYear(dt) + sep + pc.GetMonth(dt) + sep + pc.GetDayOfMonth(dt) + sep + pc.GetHour(dt) + sep + pc.GetMinute(dt) + sep + pc.GetSecond(dt);
            //clientModel.LastUpdatedBy = Context.ConnectionId;
            // Update the shape model within our broadcaster
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            context.Clients.All.AddMessageToDiv(ttp);
        }
        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);
        }
    }
}