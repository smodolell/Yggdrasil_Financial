using Microsoft.AspNetCore.Components;

namespace Yggdrasil.Site.Components.Layout;

public partial class AuthorizedLayout
{
    [Parameter] public RenderFragment? Child { get; set; }
}