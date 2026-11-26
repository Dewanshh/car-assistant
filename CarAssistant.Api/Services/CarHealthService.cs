using CarAssistant.Api.Entities;

namespace CarAssistant.Api.Services;

public class CarHealthService
{
    public int CalculateHealthScore(TelemetrySnapshot snapshot)
    {
        int score = 100;

        if (snapshot.CoolantTempC > 105) score -= 30;
        else if (snapshot.CoolantTempC > 95) score -= 10;

        if (!string.IsNullOrEmpty(snapshot.ActiveDtcCodes)) score -= 30;

        if (snapshot.FuelLevelPercent < 5) score -= 5;

        if (score < 0) score = 0;
        if (score > 100) score = 100;

        return score;
    }
}
