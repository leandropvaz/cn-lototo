using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CN.Lototo.Web.Services
{
    public class LototoAuthenticationStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser =
            new ClaimsPrincipal(new ClaimsIdentity()); // anónimo

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_currentUser));
        }

        public void SignIn(ClaimsPrincipal user)
        {
            _currentUser = user;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void SignOut()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
