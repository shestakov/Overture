namespace Overture.Core.Auth.Token
{
	public interface IAuthenticationTokenCryptography
	{
		byte[] EncryptToken(AuthenticationToken authenticationToken);
		string EncryptTokenToBase64(AuthenticationToken authenticationToken);
		AuthenticationToken DecryptToken(byte[] encryptedToken);
		AuthenticationToken DecryptTokenFromBase64(string encryptedTokenBase64);
	}
}