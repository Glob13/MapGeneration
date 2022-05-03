using System;

namespace MapCreation
{
    /**********************
     *    Inheritance     *
     **********************/
    public class Perlin : RNG
    {
        public Perlin(int inputSeed)
        {
            Seed = inputSeed;
        }
        public float Octave(float x, float y, float ratio, int octNum)
        {
            float total = 0; //the sum of all the octaves
            float frequency = 1; //affects how many sections the line or grid is split into when calculating perlin noise
            float amplitude = 1; //decides the range the result of perlin noise can be in
            float max = 0; //ensure the final output will be in the range 0-1
            for (int i = 0; i < octNum; i++)
            {
                total += Perlin2D(x * frequency, y * frequency, ratio, i.ToString()) * amplitude; //adds the sum of this octave (limited to the amplitude) to the total
                max += amplitude; //maximum possible value of the total
                amplitude *= (float)0.351; //multiply the amplitude by the 'persistence', deciding how much each octave will affect the final result
                frequency *= 2; //increase the frequency with every octave
            }
            return total / max;
        }
        private float Perlin2D(float x, float y, float ratio, string seedAppend)
        {
            float numOfGrids = 5; //splits into numOfGrids grids/segments
            x *= numOfGrids;
            y *= numOfGrids * ratio; //stops the noise from being stretched, by ensure the grids are all square
            int unitX = (int)Math.Floor(x); //grid coordinates
            int unitY = (int)Math.Floor(y);
            x %= 1; //coordinates within grid, will be in range 0-1
            y %= 1;
            int[][] gradVectors = new int[][] { new int[] { 1, 1 }, new int[] { -1, 1 }, new int[] { 1, -1 }, new int[] { -1, -1 }, new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { -1, 0 } }; //directional vectors pointing in 8 different directions
            float[][] dists = new float[][] { new float[] { x, y }, new float[] { x - 1, y }, new float[] { x, y - 1 }, new float[] { x - 1, y - 1 } }; //vectors pointing from the point (x, y) to the grid's 4 corners
            float[] grads = new float[4];
            for (int i = 0; i < 4; i++) //will randomly pick a vector from gradVectors for each corner of the grid, hashing ensures every corner will always pick the same vector
            {
                int[] gradVec;
                switch (i) //hashcode allows for each corner to be consistent with its gradVector, but also requires the seedAppend so the vectors are different between octaves
                {
                    case 0:
                        gradVec = gradVectors[Hash(unitX, unitY, Seed + seedAppend) % 8]; //bottom left corner
                        break;
                    case 1:
                        gradVec = gradVectors[Hash(unitX + 1, unitY, Seed + seedAppend) % 8]; ; //bottom right corner
                        break;
                    case 2:
                        gradVec = gradVectors[Hash(unitX, unitY + 1, Seed + seedAppend) % 8]; //top left corner
                        break;
                    default:
                        gradVec = gradVectors[Hash(unitX + 1, unitY + 1, Seed + seedAppend) % 8]; //top right corner
                        break;
                }
                //finds the dot product between random gradient vector and the distance vector
                grads[i] = Dot(gradVec[0], gradVec[1], dists[i][0], dists[i][1]);
            }
            float y1 = Lerp(grads[0], grads[1], x); //linear interpolation finds the value at the exact point we're looking at
            float y2 = Lerp(grads[2], grads[3], x);
            return (Lerp(y1, y2, y) + 1) / 2; //plus 1, divide by 2 to move the range from -1-1 to 0-1
        }
         private float Dot(float gx, float gy, float dx, float dy)
        {
            //the dot product of 2 vectors divided by the product of their magnitudes is cosine of the angle between them
            //therefore, the value of a dot product can be used to represent a direction
            return (gx * dx) + (gy * dy);
        }
        private float Lerp(float v0, float v1, float t)
        {
            //linear interpolation looks at the value at a start point and an end point, and estimates the value at a specified point in between
            return v0 + Fade(t) * (v1 - v0);
        }
        private float Fade(float t)
        {
            //without this, the lines between each grid would show up on our final result
            //the fade function curves these lines, making them unnoticeable (to see why, plot it on a graph)
            return 6 * t * t * t * t * t - 15 * t * t * t * t + 10 * t * t * t;
        }
        private int Hash(int x, int y, string seed)
        {
            /**********************
             *      Hashing       *
             **********************/
            unchecked //allows for overflow
            {
                int hash = 6217; //starts as a prime
                string input = x.ToString() + seed + y.ToString(); //unique to every x and y combination
                for (int i = 0; i < input.Length && input[i] != '\0'; i++) //foreach character in input, or until the character is null
                {
                    hash ^= input[i]; //XOR (irreversible change)
                    hash *= 1385953; //multiplied by a prime
                }
                return Math.Abs(hash);
            }
        }
    }
}