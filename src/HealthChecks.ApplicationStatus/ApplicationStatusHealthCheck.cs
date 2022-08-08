using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace HealthChecks.ApplicationStatus;

/// <summary>
/// Healthcheck that detect application graceful shutdown.
/// </summary>
public class ApplicationStatusHealthCheck : IHealthCheck, IDisposable
{
    private readonly IHostApplicationLifetime _lifetime;
    private CancellationTokenRegistration _ctRegistration = default;
    private bool IsApplicationRunning => _ctRegistration != default;

    public ApplicationStatusHealthCheck(IHostApplicationLifetime lifetime)
    {
        _lifetime = lifetime ?? throw new ArgumentNullException(nameof(IHostApplicationLifetime));
        _ctRegistration = _lifetime.ApplicationStopping.Register(OnStopping);
    }

    /// <summary>
    /// Handler that will be triggered on application stoping event.
    /// </summary>
    private void OnStopping()
    {
        Dispose();
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(IsApplicationRunning ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy());
    }

    public void Dispose()
    {
        _ctRegistration.Dispose();
        _ctRegistration = default;
    }
}
