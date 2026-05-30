using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace LocalCRM.Blazor.Components
{
    public class AuthorizePermission : ComponentBase
    {
        [Parameter] public string Permission { get; set; } = string.Empty;
        [Parameter] public RenderFragment? ChildContent { get; set; }

        [CascadingParameter] private Task<AuthenticationState>? AuthStateTask { get; set; }

        private bool _isAuthorized;

        protected override async Task OnParametersSetAsync()
        {
            if (AuthStateTask == null) return;
            var user = (await AuthStateTask).User;
            _isAuthorized = user.Claims.Any(c => c.Type == "permissions" && c.Value == Permission);
        }

        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
        {
            if (_isAuthorized && ChildContent != null)
            {
                builder.AddContent(0, ChildContent);
            }
        }
    }
}
