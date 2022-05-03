using System;
using System.Collections.Generic;
using System.Drawing;

namespace MapCreation
{
    public class MapGeneration
	{
		private RNG rand;
		private const int seedCount = 18000;
		/**********************
         *    Composition     *
         **********************/
		private SeedPoint[] seedPoints = new SeedPoint[seedCount + 1];
		private int[] mapSize =  { 1780, 1288 };
		public Bitmap Map { get; set; }
		public MapGeneration(RNG random, int waterLevel)
		{
			Map = new Bitmap(mapSize[0], mapSize[1]);
			rand = random;
			SeedSetup(waterLevel); //waterLevel represents any customisation options selected by the user
			JumpFloodAlg();
		}
		private void SeedSetup(int waterLevel)
		{
			//sets the coordinates for every seed point in the voronoi Diagram and
			//the altitude and moisture values that decide each region's colour
			Perlin noise = new Perlin(rand.Seed);
			int height = 322;
			int width = 445;
			float[,] altitude = new float[width, height];
			if (waterLevel != 10) 
			{
				//creates island shape by applying noise to an oval, and setting altitude to 0 for all points outside the resulting shapes
				double radius;
				float xoff, yoff;
				for (double theta = 0; theta < 6.28; theta += 0.007)
				{
					xoff = 1 + (float)Math.Cos(theta);
					yoff = 1 + (float)Math.Sin(theta);
					radius = 100 + 50 * noise.Octave(xoff, yoff, 1, 10);
					for (double r = 0; r < 231; r += 0.1)
					{
						if (r < radius)
						{
							int x = (int)Math.Floor(r * 1.3 * Math.Cos(theta) + 222);
							int y = (int)Math.Floor(r * Math.Sin(theta) + 161);
							if (x < 445 && x > 0 && y < 322 && y > 0)
								altitude[x, y] = noise.Octave(x / (float)width, y / (float)height, height / (float)width, 4);
							else
								break;
						}
					}
				}
			}
			else
            {
				//if landlocked option has been selected, assign altitude irrespective of an island shape
				for(int x = 0; x < width; x++)
					for(int y = 0; y< height; y++)
                    {
						altitude[x, y] = noise.Octave(x / (float)width, y / (float)height, height / (float)width, 4);
					}
            }
			noise = new Perlin(rand.Seed + 10173);
			for (int i = 0; i < seedCount + 1; i++) //assigns moisture values to each seed point
			{
				int x = rand.Next(mapSize[0]);
				int y = rand.Next(mapSize[1]);
				seedPoints[i] = new SeedPoint(x, y, altitude[x / 4, y / 4], noise.Octave(x / (float)mapSize[0], y / (float)mapSize[1], mapSize[1] / (float)mapSize[0], 4), waterLevel, true);
			}
		}
		private void JumpFloodAlg() //generates an approximation of a voronoi diagram
		{
			int step = mapSize[0] / 2;
			int[] pointLocater = new int[] { -1, 0, 1 }; //used to better loop through all the neighbouring points
			Color[] colourList = new Color[seedCount + 1];
			/**********************
             *      2D Array      *
             **********************/
			int[,] draftMap = new int[mapSize[0], mapSize[1]];
			int maxRegion = (mapSize[0] * mapSize[1]) + (seedCount);
			int[][] pointList = new int[maxRegion][]; //all the points that have been filled, grouped by colour

			for (int i = 1; i < seedPoints.Length; i++) //fills the initial seeds
			{
				pointList[i] = new int[] { seedPoints[i].X, seedPoints[i].Y };
                colourList[i] = seedPoints[i].Biome;
				draftMap[pointList[i][0], pointList[i][1]] = i;
			}
			pointList[0] = new int[] { seedCount + 1 };

			while (step > 0) //fills every point in the bitmap until step = 0
			{
				/**********************
				 *        List        *
				 **********************/
				List<int[]> tempList = new List<int[]>();
				for (int i = 1; i < maxRegion; i++) //loops through every point that has already been filled
				{
					int[] item = pointList[i];
					if (item == null)
					{
						break;
					}
					foreach (var j in pointLocater)
					{
						foreach (var k in pointLocater) //j & k exist to more efficiently find all the neighbouring points
						{
							if (j == 0 && k == 0) //so it doesn't attempt to fill itself (more efficient)
							{
								continue;
							}
							int[] focusPoint = new int[] { item[0] + step * j, item[1] + step * k }; //finds a neighbouring point (focuspoint) to the current item
							if (!(focusPoint[0] >= mapSize[0] || focusPoint[0] < 0 || focusPoint[1] >= mapSize[1] || focusPoint[1] < 0)) //ensures that focus point is within the limits of the bitmap
							{
								bool fill = false;
								int currentColour = draftMap[item[0], item[1]];
								int competitiveColour = draftMap[focusPoint[0], focusPoint[1]]; //finds if the focuspoint has already been filled
								if (competitiveColour != currentColour) //doesn't fill if the colour would remain unchanged
								{
									if (competitiveColour == 0) //fills if empty
									{
										fill = true;
										/***********************
										 *   List Operations   *
										 ***********************/
										tempList.Add(focusPoint);
									}
									else
									{
										//if the distance between the focuspoint and it's seed of origin is less than the focuspoint and it's current colour's seed of origin, then replace the pixel's colour.
										int[] competitionPoint = pointList[competitiveColour];
										if (PointDistance(focusPoint[0] - competitionPoint[0], focusPoint[1] - competitionPoint[1], focusPoint[0] - pointList[currentColour][0], focusPoint[1] - pointList[currentColour][1]))
										{
											fill = true;
										}
									}
								}
								if (fill)
								{
									draftMap[focusPoint[0], focusPoint[1]] = currentColour;
								}
							}
						}
					}
				}
				foreach (int[] item in tempList)
				{
					pointList[pointList[0][0]] = item;
					pointList[0][0] = pointList[0][0] + 1; //allows pointlist[0][0] to hold the index value of the next empty cell in the array
				}
				step = step != 1 ? (int)Math.Floor((double)step / 2) : 0;
			}
			for (int i = 0; i < mapSize[0]; i++)
			{
				for (int j = 0; j < mapSize[1]; j++)
				{
					Map.SetPixel(i, j, colourList[draftMap[i, j]]);
				}
			}
		}
		private bool PointDistance(int x1, int y1, int x2, int y2) //calculates and compares distances between points and their origins
		{
			int distance1 = x1 * x1 + y1 * y1;
			int distance2 = x2 * x2 + y2 * y2;
			return distance1 >= distance2;
		}
    }
}
