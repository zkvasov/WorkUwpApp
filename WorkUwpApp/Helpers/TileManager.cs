using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace WorkUwpApp.Helpers
{
    public static class TileManager
    {
        public static async Task SendTileNotificationAsync(string appName, string subject, string body)
        {

            //string from = "Jennifer Parker";
            //string subject = "Photos from our trip";
            //string body = "Check out these awesome photos I took while in New Zealand!";


            // Construct the tile content
            TileContent content = GenerateTileContent(appName, subject, body);

            AppListEntry entry = (await Package.Current.GetAppListEntriesAsync())[0];
            bool isPined = await StartScreenManager.GetDefault().ContainsAppListEntryAsync(entry);
            if (!isPined)
            {
                await StartScreenManager.GetDefault().RequestAddAppListEntryAsync(entry);
            }

            TileNotification notification = new TileNotification(content.GetXml());
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        private static TileContent GenerateTileContent(string appName, string subject, string body)
        {
            // Construct the tile content
            return new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = new TileBinding()
                    {

                        
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveText()
                    {
                        Text = appName
                    },
                   
                    new AdaptiveText()
                    {
                        Text = subject,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    },

                    new AdaptiveText()
                    {
                        Text = body,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    }
                }
                        }
                    },

                    TileWide = new TileBinding()
                    {

                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveText()
                    {
                        Text = appName,
                        HintStyle = AdaptiveTextStyle.Subtitle
                    },

                    new AdaptiveText()
                    {
                        Text = subject,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    },

                    new AdaptiveText()
                    {
                        Text = body,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    }
                }
                        }
                    }
                }
            };
        }
    }
}
