namespace Watch.Manager.ServiceDefaults;

using Microsoft.Extensions.Configuration;

/// <summary>
///     Extension methods for <see cref="IConfiguration" />.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    ///     Gets a required value from the configuration.
    /// </summary>
    /// <param name="configuration">The configuration to get the value from.</param>
    /// <param name="name">The name of the configuration value to get.</param>
    /// <returns>The value of the configuration.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the configuration value is missing.</exception>
    public static string GetRequiredValue(this IConfiguration configuration, string name) =>
            configuration[name] ?? throw new InvalidOperationException($"Configuration missing value for: {(configuration is IConfigurationSection s ? s.Path + ":" + name : name)}");
}
