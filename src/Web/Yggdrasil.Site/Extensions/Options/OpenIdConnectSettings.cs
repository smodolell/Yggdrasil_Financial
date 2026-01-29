namespace Yggdrasil.Site.Extensions.Options;

public class OpenIdConnectSettings
{
    public string Authority { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string ResponseType { get; set; } = "code";
    public bool UsePkce { get; set; } = true;
    public List<string> Scopes { get; set; } = new() { "openid", "profile", "email" };
    public bool SaveTokens { get; set; } = true;
    public bool GetClaimsFromUserInfoEndpoint { get; set; } = true;
    public string NameClaimType { get; set; } = "name";
    public string RoleClaimType { get; set; } = "role";
    public string CallbackPath { get; set; } = "/signin-oidc";
}
