Namespace Entities

    ''' <summary>
    ''' ESEMPIO: tabella legacy "tblOrdini", in relazione N:1 con tblClienti.
    ''' </summary>
    Public Class Ordine

        Public Property Id As Integer
        Public Property ClienteId As Integer
        Public Property DataOrdine As Date
        ''' <summary>Campo Valuta (Currency) in Access, decimal(19,4) in SQL Server.</summary>
        Public Property Importo As Decimal
        ''' <summary>Campo Memo / Testo lungo in Access, nvarchar(max) in SQL Server.</summary>
        Public Property Note As String

        Public Property Cliente As Cliente

    End Class

End Namespace
