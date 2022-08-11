﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DiscordRPC;

namespace Bloxstrap.Helpers
{
	internal class DiscordRichPresence : IDisposable
	{
		readonly DiscordRpcClient RichPresence = new("1005469189907173486");

		public async Task<bool> SetPresence(string placeId)
        {
			string placeName;
			string placeThumbnail;
			string creatorName;

			// null checking could probably be a lot more concrete here
			using (HttpClient client = new())
			{
				JObject placeInfo = await Utilities.GetJson($"https://economy.roblox.com/v2/assets/{placeId}/details");

				placeName = placeInfo["Name"].Value<string>();
				creatorName = placeInfo["Creator"]["Name"].Value<string>();

				JObject thumbnailInfo = await Utilities.GetJson($"https://thumbnails.roblox.com/v1/places/gameicons?placeIds={placeId}&returnPolicy=PlaceHolder&size=512x512&format=Png&isCircular=false");

				if (thumbnailInfo["data"] is null)
					return false;

				placeThumbnail = thumbnailInfo["data"][0]["imageUrl"].Value<string>();
			}

			RichPresence.Initialize();

			RichPresence.SetPresence(new RichPresence()
			{
				Details = placeName,
				State = $"by {creatorName}",
				Timestamps = new Timestamps() { Start = DateTime.UtcNow },

				Assets = new Assets()
				{
					LargeImageKey = placeThumbnail,
					LargeImageText = placeName,
					SmallImageKey = "bloxstrap",
					SmallImageText = "Rich Presence provided by Bloxstrap"
				},

				Buttons = new DiscordRPC.Button[]
				{
					new DiscordRPC.Button()
					{
						Label = "Play",
						Url = $"https://www.roblox.com/games/start?placeId={placeId}&launchData=%7B%7D"
					},

					new DiscordRPC.Button() 
					{ 
						Label = "View Details", 
						Url = $"https://www.roblox.com/games/{placeId}"
					}
				}
			});

			return true;
		}

		public void Dispose()
		{
			RichPresence.Dispose();
		}
	}
}