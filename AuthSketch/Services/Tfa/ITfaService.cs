using AuthSketch.Models.Tfa;

namespace AuthSketch.Services.Tfa;

public interface ITfaService
{
    Task<TfaResponse> EnableTfaAsync();
    Task DisableTfaAsync(DisableTfaRequest request);
    Task SendCodeOnEmailAsync();
}