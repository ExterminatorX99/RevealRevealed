namespace RevealRevealed.Items;

public class RevealRevealed : ModItem
{
	public override string Texture => "Terraria/Images/Map_4";

	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("RevealArea");
		Tooltip.SetDefault("Reveals map around light torches\nMade to recover lost map data");
	}

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 30;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.useAnimation = 30;
		Item.useTime = 30;
		Item.useStyle = ItemUseStyleID.HoldUp;
	}

	public override bool CanUseItem(Player player)
	{
		static byte LightCalc(int x, int y)
		{
			int light = 230 - (int) Math.Sqrt(x * x + y * y) * 14;
			return (byte) MathHelper.Clamp(light, byte.MinValue, byte.MaxValue);
		}

		var lightSources = new Dictionary<(int x, int y), byte>();
		const int radius = 12;

		for (int x = 0; x < Main.maxTilesX; ++x)
		for (int y = 0; y < Main.maxTilesY; ++y)
			if (Main.tile[x, y].type == TileID.Torches)
				for (int addX = x - radius; addX <= x + radius; addX++)
				for (int addY = y - radius; addY <= y + radius; addY++)
				{
					byte light = LightCalc(x - addX, y - addY);
					if (lightSources.ContainsKey((addX, addY)))
					{
						if (light > lightSources[(addX, addY)])
							lightSources[(addX, addY)] = light;
					}
					else
					{
						lightSources[(addX, addY)] = light;
					}
				}

		foreach (((int x, int y), byte light) in lightSources)
			if (WorldGen.InWorld(x, y))
				Main.Map.Update(x, y, light);

		Main.refreshMap = true;
		return true;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.DirtBlock, 10)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
