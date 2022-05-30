using System;
using Xunit;
using Shop.Repository;

namespace Shop.Tests
{
	public class SecurityTest
	{
		private UserRepository _userRepository = new UserRepository();

		[Fact]
		public void LoginCorrect()
		{
			var result = _userRepository.Login("Joni", "Smit");
			Assert.True(result, "login doesnt work");
		}
	}
}
