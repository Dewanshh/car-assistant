using System.Globalization;
using System.Net.Http.Json;

namespace ObdReplay;

public class Program
{
    // CONFIGURE THESE:
    // Here we need to update the JWT Token as it is required on the other side data is now easily send through it
    // Next we need to create a data veification via chatgpt and then show everything on the dashboard!!!
    //Also chekc if we have one odb driver than how we can get the live data from it and how we check its faa =
    //Here we are passing car guid
    private const string TelemetryApiUrl = "http://localhost:5125/api/cars/9649a646-4213-4be7-9027-dda93843e46d/e0d4c052-b3cb-40ec-af6d-bbd7a79762dd/telemetry";
    private const string JwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI2MGMzNjM4Yi1lY2M3LTQwYjktYTNkOS0xODI4NTY1M2M5MDciLCJlbWFpbCI6ImRAZy5jb20iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYwYzM2MzhiLWVjYzctNDBiOS1hM2Q5LTE4Mjg1NjUzYzkwNyIsImV4cCI6MTc2NDc5MzcyNiwiaXNzIjoiQ2FyQXNzaXN0YW50IiwiYXVkIjoiQ2FyQXNzaXN0YW50VXNlcnMifQ.TcTMV4TOKp_iftCCE16W11rThaOemJV7celjIdI6eDg";
    private const string GvretLogFilePath = "/Users/dewanshjangir/Desktop/CarAssistant/CarAssistant/CarAssistant.Api/Data/savvy_dummy_log.csv";

    public static async Task Main()
    {
        Console.WriteLine("Starting GVRET replay -> Telemetry API bridge...");

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtToken);

        var state = new TelemetryState();

        using var reader = new StreamReader(GvretLogFilePath);

        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            line = line.Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.StartsWith(";") || line.StartsWith("#")) continue;

            try
            {
                var frame = ParseGvretLine(line);
                if (frame == null) continue;

                ApplyFrameToState(frame, state);

                // Send current snapshot to API
                var payload = new TelemetryPayload
                {
                    Rpm = state.Rpm,
                    SpeedKph = state.SpeedKph,
                    CoolantTempC = state.CoolantTempC,
                    FuelLevelPercent = state.FuelLevelPercent,
                    ActiveDtcCodes = state.ActiveDtcCodes
                };

                var response = await httpClient.PostAsJsonAsync(TelemetryApiUrl, payload);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API error: {(int)response.StatusCode} {response.ReasonPhrase}");
                }
                else
                {
                    Console.WriteLine(
                        $"Sent: RPM={payload.Rpm}, Speed={payload.SpeedKph}, " +
                        $"Coolant={payload.CoolantTempC}, Fuel={payload.FuelLevelPercent}, DTC={payload.ActiveDtcCodes}");
                }

                // Simulate real-time replay
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing line: {line}");
                Console.WriteLine(ex.Message);
            }
        }

        Console.WriteLine("Replay finished.");
    }

    // ---------- Models ----------

    public record TelemetryPayload
    {
        public int Rpm { get; init; }
        public double SpeedKph { get; init; }
        public double CoolantTempC { get; init; }
        public double FuelLevelPercent { get; init; }
        public string? ActiveDtcCodes { get; init; }
    }

    public class TelemetryState
    {
        public int Rpm { get; set; }
        public double SpeedKph { get; set; }
        public double CoolantTempC { get; set; }
        public double FuelLevelPercent { get; set; }
        public string? ActiveDtcCodes { get; set; }
    }

    public record GvretFrame(
        double Timestamp,
        int Id,
        int Ide,
        int Dlc,
        byte[] Data);

    // ---------- Parsing ----------

    private static GvretFrame? ParseGvretLine(string line)
    {
        // Example: 0.001000,7E8,0,8,03,41,0C,1A,F8,00,00,00
        var parts = line.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 4) return null;

        double timestamp = double.Parse(parts[0], CultureInfo.InvariantCulture);
        int id = int.Parse(parts[1], NumberStyles.HexNumber);
        int ide = int.Parse(parts[2], CultureInfo.InvariantCulture);
        int dlc = int.Parse(parts[3], CultureInfo.InvariantCulture);

        var dataBytes = new List<byte>();
        for (int i = 4; i < parts.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(parts[i])) continue;
            dataBytes.Add(Convert.ToByte(parts[i], 16));
        }

        // Pad if less than DLC
        while (dataBytes.Count < dlc)
        {
            dataBytes.Add(0x00);
        }

        return new GvretFrame(timestamp, id, ide, dlc, dataBytes.ToArray());
    }

    // ---------- Apply frame -> State ----------

    private static void ApplyFrameToState(GvretFrame frame, TelemetryState state)
    {
        // Only care about ECU response ID 0x7E8
        if (frame.Id != 0x7E8) return;
        if (frame.Data.Length < 3) return;

        var d = frame.Data;

        // DTC frame: 0x43 in data[1]
        if (d[1] == 0x43)
        {
            state.ActiveDtcCodes = DecodeDtcFrame(d);
            return;
        }

        // Normal OBD response to Mode 01: 0x41 in data[1]
        if (d[1] != 0x41) return;

        byte pid = d[2];

        switch (pid)
        {
            case 0x0C: // RPM
            {
                if (frame.Dlc < 6) return;
                byte A = d[3];
                byte B = d[4];
                int rpm = ((A * 256) + B) / 4;
                state.Rpm = rpm;
                break;
            }

            case 0x0D: // Speed
            {
                if (frame.Dlc < 5) return;
                byte A = d[3];
                state.SpeedKph = A;
                break;
            }

            case 0x05: // Coolant temp
            {
                if (frame.Dlc < 5) return;
                byte A = d[3];
                state.CoolantTempC = A - 40;
                break;
            }

            case 0x2F: // Fuel level
            {
                if (frame.Dlc < 5) return;
                byte A = d[3];
                // Per OBD-II: Fuel level % = 100/255 * A, but our log uses 0-100 directly, so:
                state.FuelLevelPercent = A; 
                break;
            }

            default:
                // Other PIDs ignored for now
                break;
        }
    }

    // ---------- DTC decoding ----------

    private static string? DecodeDtcFrame(byte[] data)
    {
        // Example: 06 43 01 03 00 00 00 00
        // Service 0x43, then pairs of DTC bytes
        if (data.Length < 4) return null;

        var dtcs = new List<string>();

        // Start from index 2 for DTC bytes
        for (int i = 2; i + 1 < data.Length; i += 2)
        {
            byte b1 = data[i];
            byte b2 = data[i + 1];

            if (b1 == 0 && b2 == 0) continue;

            string dtc = DecodeSingleDtc(b1, b2);
            dtcs.Add(dtc);
        }

        return dtcs.Count > 0 ? string.Join(",", dtcs) : null;
    }

    private static string DecodeSingleDtc(byte b1, byte b2)
    {
        // From OBD-II spec:
        // First two bits of b1: system (P, C, B, U)
        char system = (b1 >> 6) switch
        {
            0 => 'P',
            1 => 'C',
            2 => 'B',
            3 => 'U',
            _ => 'P'
        };

        int digit1 = (b1 >> 4) & 0x3;
        int digit2 = b1 & 0x0F;
        int digit3 = (b2 >> 4) & 0x0F;
        int digit4 = b2 & 0x0F;

        return $"{system}{digit1}{digit2}{digit3}{digit4}";
    }
}
