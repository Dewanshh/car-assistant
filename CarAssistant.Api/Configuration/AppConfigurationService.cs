using CarAssistant.Api.Configuration;

public interface IAppConfigurationService
{
    JwtSettings GetJwtSettings();
}

public class AppConfigurationService : IAppConfigurationService
{
    private readonly IConfiguration _configuration;

    public AppConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public JwtSettings GetJwtSettings()
    {
        var settings = new JwtSettings();
        _configuration.GetSection("Jwt").Bind(settings);
        return settings;
    }
}
