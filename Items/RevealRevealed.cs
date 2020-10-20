using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RevealRevealed.Items
{
	internal class RevealRevealed : ModItem
	{
		public override string Texture => "Terraria/Map_4";

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("RevealArea");
			Tooltip.SetDefault("Reveals map around light torches\nMade to recover lost map data");
		}

		public override void SetDefaults() {
			item.width = 20;
			item.height = 20;
			item.value = 0;
			item.rare = ItemRarityID.White;
			item.useAnimation = 30;
			item.useTime = 30;
			item.useStyle = ItemUseStyleID.HoldingUp;
		}

		public override bool UseItem(Player player) {
			byte lightCalc(int x, int y) {
				int light = 230 - (int)Math.Sqrt(x * x + y * y) * 14;
				return (byte)MathHelper.Clamp(light, 0, 255);
			}

			var lightSources = new Dictionary<(int x, int y), byte>();
			const int radius = 12;

			for (int x = 0; x < Main.maxTilesX; ++x)
			for (int y = 0; y < Main.maxTilesY; ++y)
				if (Main.tile[x, y].type == TileID.Torches)
					for (int addX = x - radius; addX <= x + radius; addX++)
					for (int addY = y - radius; addY <= y + radius; addY++) {
						byte light = lightCalc(x - addX, y - addY);
						if (lightSources.ContainsKey((addX, addY))) {
							if (light > lightSources[(addX, addY)])
								lightSources[(addX, addY)] = light;
						}
						else {
							lightSources[(addX, addY)] = light;
						}
					}

			foreach (KeyValuePair<(int x, int y), byte> kvp in lightSources) {
				(int x, int y) = kvp.Key;
				byte light = kvp.Value;
				if (WorldGen.InWorld(x, y))
					Main.Map.Update(x, y, light);
			}

			Main.refreshMap = true;
			return true;
		}

		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
