namespace Watch.Manager.Service.Analyse.Options;

/// <summary>
///     Represents configuration options for AI-related settings.
/// </summary>
/// <remarks>
///     This class provides a container for settings related to AI functionality,  including specific
///     configurations for OpenAI.
/// </remarks>
public sealed class AIOptions
{
    /// <summary>Settings related to the use of OpenAI.</summary>
    public OpenAIOptions OpenAI { get; set; } = new();
}
