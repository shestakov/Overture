using System;

namespace Overture.Core.Auth.Token
{
	public class AuthenticationToken
	{
		public AuthenticationToken(Guid userId)
		{
			UserId = userId;
			ValidThrough = DateTime.UtcNow.Add(authenticationTokenValidityPeriod);
		}

		private AuthenticationToken(Guid userId, DateTime validThrough)
		{
			UserId = userId;
			ValidThrough = validThrough;
		}

		public Guid UserId { get; private set; }
		private DateTime ValidThrough { get; set; }

		public bool IsValid { get { return ValidThrough > DateTime.UtcNow; } }

		public byte[] Serialize()
		{
			var result = new byte[16 + 8];
			Array.Copy(UserId.ToByteArray(), result, 16);
			Array.Copy(BitConverter.GetBytes(ValidThrough.Ticks), 0, result, 16, 8);
			return result;
		}

		public static AuthenticationToken Deserialize(byte[] token)
		{
			if (token.Length != 16 + 8)
				throw new ArgumentException("Wrong token length", "token");
			var userId = new byte[16];
			Array.Copy(token, 0, userId, 0, 16);
			return new AuthenticationToken(new Guid(userId), new DateTime(BitConverter.ToInt64(token, 16), DateTimeKind.Utc));
		}

		private static readonly TimeSpan authenticationTokenValidityPeriod = new TimeSpan(30, 0, 0, 0);
	}
}