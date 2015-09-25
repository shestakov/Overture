using System;

namespace Overture.Core.Auth.Users
{
	public interface IUserManager<TUser, TUserActivationRequest, in TCreateUserForm, in TUpdateUserForm>
		where TUser : class, IUser
		where TUserActivationRequest : class, IUserActivationRequest, new()
		where TCreateUserForm : class, IUserForm
	{
		TUser CreateUserAndActivationRequest(TCreateUserForm form, Uri activateUserUrl);
		TUser CreateActiveUser(TCreateUserForm form);
		TUser ActivateUser(Guid userActivationRequestId);
		
		void UpdateUserDetails(Guid userId, TUpdateUserForm form);
		UserActivationRequestValidationResult<TUserActivationRequest, TUser> ValidateUserActivationRequest(Guid userActivationRequestId);
	}


	public class UserActivationRequestValidationResult<TUserActivationRequest, TUser>
	{
		public UserActivationRequestValidationResult(TUserActivationRequest userActivationRequest, TUser user)
		{
			UserActivationRequest = userActivationRequest;
			User = user;
		}

		public TUserActivationRequest UserActivationRequest { get; private set; }
		public TUser User { get; private set; }
	}
}