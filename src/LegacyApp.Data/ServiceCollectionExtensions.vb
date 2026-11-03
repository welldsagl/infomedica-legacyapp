Imports System.Runtime.CompilerServices
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.DependencyInjection

''' <summary>
''' Registrazione del layer dati in un host con Dependency Injection (Web API, worker, WinForms con Generic Host).
''' </summary>
Public Module ServiceCollectionExtensions

    <Extension>
    Public Function AddAppData(services As IServiceCollection, options As DataOptions) As IServiceCollection
        ArgumentNullException.ThrowIfNull(services)
        ArgumentNullException.ThrowIfNull(options)

        services.AddSingleton(options)

        ' AppDbContext e' registrato "scoped": un'istanza per richiesta HTTP.
        services.AddDbContext(Of AppDbContext)(
            Sub(builder)
                builder.UseAppDatabase(options.Provider, options.ConnectionString, options.JetDualTable)
                If options.EnableSensitiveDataLogging Then
                    builder.EnableSensitiveDataLogging()
                End If
            End Sub)

        Return services
    End Function

End Module
