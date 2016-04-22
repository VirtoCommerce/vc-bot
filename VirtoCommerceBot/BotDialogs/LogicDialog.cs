using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using VirtoCommerce.Client;
using VirtoCommerce.Client.Api;
using VirtoCommerce.Client.Model;
using System.Configuration;
using Microsoft.Bot.Builder.FormFlow;

namespace VirtoCommerceBot.BotDialogs
{
    // https://api.projectoxford.ai/luis/v1/application?id=7689070f-9717-4b61-918d-88319c7b2936&subscription-key=b280d69a16bb4d87b46cf0bfcff16ca9
    [LuisModel("7689070f-9717-4b61-918d-88319c7b2936", "b280d69a16bb4d87b46cf0bfcff16ca9")]
    [Serializable]
    public class LogicDialog : LuisDialog<object>
    {
        public const string DefaultAlarmWhat = "default";

        [NonSerialized]
        VirtoCommerce.Client.Client.Configuration _config = null;

        public VirtoCommerce.Client.Client.Configuration Config
        {
            get
            {
                if(_config == null)
                {
                    var baseUrl = ConfigurationManager.ConnectionStrings["VirtoCommerceBaseUrl"].ConnectionString;
                    var apiAppId = ConfigurationManager.AppSettings["vc-public-ApiAppId"];
                    var apiSecretKey = ConfigurationManager.AppSettings["vc-public-ApiSecretKey"];
                    var client = new HmacApiClient(baseUrl, apiAppId, apiSecretKey);
                    _config = new VirtoCommerce.Client.Client.Configuration(client);
                }

                return _config;
            }
        }

        public bool TryFindObjectType(LuisResult result, out string type)
        {
            string what;
            EntityRecommendation entityType;
            if (result.TryFindEntity(Entity_Type, out entityType))
            {
                what = entityType.Entity;
            }
            else
            {
                type = null;
                return false;
            }

            type = what;
            return true;
        }

        public const string Entity_Type = "InfoType";

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("LookupInfo")]
        public async Task Lookup(IDialogContext context, LuisResult result)
        {
            string objectType;
            if (TryFindObjectType(result, out objectType))
            {
                if (objectType.Equals("order", StringComparison.OrdinalIgnoreCase))
                {
                    var orderClient = new OrderModuleApi(Config);

                    var criteria = new VirtoCommerceDomainOrderModelSearchCriteria();
                    criteria.CustomerId = "ae0e9e44-a1ca-4527-8319-0e92b0eb16f9";

                    var orders = await orderClient.OrderModuleSearchAsync(criteria);

                    if (orders.TotalCount == 0)
                    {
                        await context.PostAsync("There are no orders available, do you want to **[{create one}](http://demo.virtocommerce.com})**?");
                    }
                    else
                    {
                        var order = orders.CustomerOrders[0];
                        await context.PostAsync(Format(order));
                    }
                }
                else if (objectType.Equals("user", StringComparison.OrdinalIgnoreCase))
                {
                    string userName;
                    string userId;
                    context.UserData.TryGetValue("id", out userId);

                    if (context.UserData.TryGetValue("name", out userName))
                        await context.PostAsync($"name: {userName}, id: {userId}");
                    else
                        await context.PostAsync("no user name found");
                }
                else if (objectType.Equals("contact", StringComparison.OrdinalIgnoreCase))
                {
                    string userName;
                    string userId;
                    context.UserData.TryGetValue("id", out userId);

                    if (context.UserData.TryGetValue("name", out userName))
                        await context.PostAsync($"name: {userName}, id: {userId}");
                    else
                        await context.PostAsync("no user name found");
                }
            }
            else
            {
                string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
                await context.PostAsync(message);
            }
            context.Wait(MessageReceived);
        }

        enum LocationOptions { UnitedStates, Europe };

        enum ContactOptions { Email = 1, Call, Visit };

        enum QuestionOptions { Billing, Demo, Support };


        [LuisIntent("ContactInfo")]
        public async Task ProcessContactInfo(IDialogContext context, LuisResult result)
        {
            var contactForm = Chain.From(() => FormDialog.FromForm(ContactForm.BuildForm, FormOptions.PromptInStart));
            context.Call(contactForm, ContactFormComplete);
        }

        [LuisIntent("Hello")]
        public async Task ProcessHello(IDialogContext context, LuisResult result)
        {
            string message = $"Hi there, this is a sample bot you can use to communicate with virto commerce, type **help** to learn more about different ways you can interact. I'm also constantly learning new things, so come back soon!";
            await context.PostAsync(message);
        }

        [LuisIntent("Help")]
        public async Task ProcessHelp(IDialogContext context, LuisResult result)
        {
            string message = $"Using this bot you can lookup your last order from the demo site (**where is my order?**) or find out contact information for virto commerce (**where are you located?**). This bot is very easy to extend and adopt to your needs and full source code is available, just ask!";
            await context.PostAsync(message);
        }

        private async Task ContactFormComplete(IDialogContext context, IAwaitable<ContactForm> result)
        {

            ContactForm form = null;
            try
            {
                form = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("You canceled the form!");
                return;
            }

            context.Wait(MessageReceived);

            var address = string.Empty;

            var messageText = String.Empty;

            if(form != null)
            {
                if(form.Location == LocationOptions.Europe)
                {
                    messageText = "**Laisvės pr. 79E Vilnius, Lithuania 06122**.";
                    if (form.Media == ContactOptions.Call)
                        messageText += " Call us at [+7 (921) 710 36 67](tel:79217103667).";
                }
                else
                {
                    messageText = "[**20945 Devonshire St Suite 102 Los Angeles, California**](https://www.google.com/maps/place/20945+Devonshire+St+Suite+102,+Los+Angeles,+CA/) \n\r ![Location](http://maps.googleapis.com/maps/api/staticmap?center=20945+Devonshire+St+Suite+102,+Los+Angeles,+CA&zoom=18&scale=2&size=600x300&maptype=roadmap&format=png&visual_refresh=true&markers=size:mid%7Ccolor:0xff0000%7Clabel:1%7C20945+Devonshire+St+Suite+102,+Los+Angeles,+CA)";
                    if (form.Media == ContactOptions.Call)
                        messageText += " Call us at [(800) 980-5288](tel:18009805288) or [(323) 570-5588](tel:13235705588).";
                }

                if (form.Media == ContactOptions.Email)
                {
                    if (form.Reason == QuestionOptions.Billing)
                        messageText += " Email us at [billing@virtocommerce.com](emailto:billing@virtocommerce.com).";
                    else if (form.Reason == QuestionOptions.Demo)
                        messageText += " Email us at [sales@virtocommerce.com](emailto:sales@virtocommerce.com).";
                    else if (form.Reason == QuestionOptions.Support)
                        messageText += " Email us at [support@virtocommerce.com](emailto:support@virtocommerce.com).";
                }
            }
            else
            {
                await context.PostAsync("Form returned empty response!");
            }

            var text = $"Our office located at {messageText}";
            await context.PostAsync(text);

            context.Wait(MessageReceived);
        }

        [Serializable]
        class ContactForm
        {
            [Prompt("Where are you located? {||}")]
            public LocationOptions? Location;

            [Prompt("How would you like to contact us? {||}")]
            public ContactOptions Media;

            [Prompt("What is the reason? {||}")]
            public QuestionOptions? Reason;

            public static IForm<ContactForm> BuildForm()
            {
                return new FormBuilder<ContactForm>()
                    .Field(nameof(ContactForm.Reason))
                        .Field(nameof(ContactForm.Location))
                        .Field(nameof(ContactForm.Media))
                        .Build();
            }
        };

        private string Format(VirtoCommerceOrderModuleWebModelCustomerOrder order)
        {
            var ret = $"Your order **[{order.Number}](http://demo.virtocommerce.com/orders/{order.Number})** is now **{order.Status}**";
            return ret;
        }

        public LogicDialog(ILuisService service = null)
            : base(service)
        {
        }
    }
}


