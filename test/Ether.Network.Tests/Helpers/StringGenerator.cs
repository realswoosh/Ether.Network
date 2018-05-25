using System;
using System.Linq;

namespace Ether.Network.Tests.Helpers
{
    public static class Helper
    {
        private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		private const string CharactersAll = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789가나다라마바사아자차카타파하申東明";
		/// <summary>
		/// Generates a random string.
		/// </summary>
		/// <param name="count">Length of the string.</param>
		/// <returns>Generated string</returns>
		public static string GenerateRandomString(int count = 8)
        {
            var random = new Random();

            return new string(
                Enumerable.Repeat(Characters, count)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
        }

		public static string GenerateRandomStringAllCharacter(int count = 8)
		{
			var random = new Random();

			return new string(
				Enumerable.Repeat(CharactersAll, count)
						  .Select(s => s[random.Next(s.Length)])
						  .ToArray());
		}
    }
}
