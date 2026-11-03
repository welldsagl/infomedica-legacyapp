''' <summary>
''' Database fisico su cui lavora l'applicazione.
''' Il codice di dominio e le query LINQ non devono dipendere da questo valore:
''' si sceglie solo in configurazione.
''' </summary>
Public Enum DatabaseProvider
    Access = 0
    SqlServer = 1
End Enum
