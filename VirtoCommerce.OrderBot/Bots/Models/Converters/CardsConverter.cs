using Microsoft.Bot.Schema;
using System;
using System.Linq;
using VirtoCommerce.OrderBot.Infrastructure;

namespace VirtoCommerce.OrderBot.Bots.Models.Converters
{
    public static class CardsConverter
    {
        public static HeroCard[] GetCards(this Product[] products)
        {
            return products
                .Select(p => new HeroCard
                {
                    Images = new[]
                    {
                        new CardImage(p.ImageUrl)
                    },
                    Title = $"{p.Name}{Environment.NewLine}SKU: {p.Code}{Environment.NewLine}Price: **{p.Price} {p.Currency}**",
                    Buttons = new[]
                    {
                        new CardAction
                            {Title = "Add to cart", Type = ActionTypes.ImBack, Value = $"{BotCommands.AddToCart}{p.Code}"}
                    }
                })
                .ToArray();
        }

        public static HeroCard[] GetCards(this LineItem[] lineItems)
        {
            return lineItems
                .Select(l => new HeroCard
                {
                    Images = new[]
                    {
                        new CardImage(l.ImgUrl)
                    },
                    Title = $"{l.Name}{Environment.NewLine}SKU: {l.Code}{Environment.NewLine}Price: **{l.Price * l.Quantity} {l.Currency}**{Environment.NewLine}Quantity: {l.Quantity}",
                })
                .ToArray();
        }
    }
}
