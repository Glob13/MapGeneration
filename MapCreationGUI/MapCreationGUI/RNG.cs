using System;

namespace MapCreation
{
    public class RNG
	{
		public int Seed { get; set; }
		public RNG(string inputSeed = "")
		{
			if (inputSeed == "") //if no seed is given, generate a random one
			{
				Seed = SeedGen();
			}
			else if (int.TryParse(inputSeed, out int output)) //if the seed is an integer, leave it in that form
			{
				Seed = output;
			}
			else //converts seed strings to numbers in a irreversible way, wherein an inputted string will always return the same number
			{
				Seed = Hash(inputSeed);
			}
		}
		private int SeedGen()
		{
			TimeSpan x = DateTime.Now - DateTime.Today.AddDays(-1); //uses a truly random number - the current time of day in milliseconds - as the seed
            return (int)x.TotalMilliseconds;
		}
		public int Next(int range) //a simple Pseudo Random Number Generator
		{
			/*PRNGs will always repeat themselves eventually, but by using large prime numbers, the
			 number of iterations before it reaches that point becomes large enough to be disregarded*/

			int output = Math.Abs(Seed % range); // gets a random number between 0 and the specified range
            Seed %= 38912897; //modulus of a coprime (the product of 2 primes)
            Seed *= 132049; //multiply by a prime
            Seed += 11213; //plus a prime 
            return output;
        }
		protected int Hash(string input)
        {
			/**********************
             *      Hashing       *
             **********************/
			unchecked //allows for overflow
			{
				int hash = 6217; //starts as a prime

				for (int i = 0; i < input.Length && input[i] != '\0'; i++) //foreach character in input, or until the character is null
                {
					hash ^= input[i]; //XOR (irreversible change)
					hash *= 1385953; //multiplied by a prime
				}
				return hash;
            }
        }
    }
}