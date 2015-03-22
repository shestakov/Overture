namespace Overture.Core.Auth.Utility
{
	public interface IAuthEmailSender
	{
		void SendActivateUserEmail(string email, string url);
		void SendPasswordResetEmail(string email, string url);
		void SendUserInvitation(string email, string invitationUrl, string organizationTitle);
		void SendAcceptInvitationEmail(string email, string userName, string organizationName, string url);
	}
}