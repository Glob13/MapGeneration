using System.Drawing;

namespace MapCreation
{
    public class SeedPoint
	{
		public Color Biome { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public SeedPoint(int x, int y, float height, float moisture, int waterLevel, bool foundingSeed = false)
		{
			X = x;
			Y = y;
			if (foundingSeed) //decides colour of the initial seed points
            {
                Biome = AssignBiome((int)(height * 255) , (int)(moisture * 255), waterLevel); //height and moisture are mapped to the range 0-255 for simplicity while working with rgb
            }
        }
		private Color AssignBiome(int height, int moisture, int waterLevel) //assigns colours representing different biomes based on height and moisture at the seedpoint (what biome the colour actually suggests is subjective and up to the user)
        {
			Color biome;
			if (height + waterLevel < 100) //water level
            {
				biome = Color.RoyalBlue;
            }
			else if (height + waterLevel < 125)
            {
				height += waterLevel;
				if (moisture < 128)//sand/plain
				{
					biome = Color.FromArgb(255 - (moisture/3),height < 150 ? height + 105 : 255, 128);
                }
				else
                {
					//marsh/swamp
					biome = Color.FromArgb(74 + (height - 100), 70 + (moisture / 3), 70 - (moisture/9)); //have to increase the variation
				}
            }
			else if (height + waterLevel * 9 / 10< 175)
			{
				if (moisture > 108) //forest
				{
					biome = Color.FromArgb(height/4, moisture/2 + 20, height/4);
				}
				else //hills
                {
					biome = Color.FromArgb(75 + (moisture / 2), 46 + (moisture / 2), 4);
				}
            }
			else if (height + waterLevel * 4 / 5 < 190) //mountains
            {
				int val = (height - 175) + moisture;
				val = val > 255 ? 255 : val;
				biome = Color.FromArgb(val, val, val);
            }
            else
            {
				height += waterLevel;
				if (moisture > 145) //snow
				{
					int val = (height - 175) + moisture + 50;
					val = val > 255 ? 255 : val;
					biome = Color.FromArgb(val, val, val);
				}
				else //lava
                {
					biome = Color.FromArgb(255, moisture, 0);
                }
			}
			return biome;
        }
	}
}
