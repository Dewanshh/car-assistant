namespace CarAssistant.Api.Configuration;

public interface IAppConfigurationService
{
    JwtSettings GetJwtSettings();
    AppSettings GetAppSettings();
}
