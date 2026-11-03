Imports LegacyApp.Api.Contracts
Imports LegacyApp.Data
Imports LegacyApp.Data.Entities
Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.EntityFrameworkCore

Namespace Controllers

    ''' <summary>
    ''' CRUD REST su tblClienti. Usa solo LINQ "portabile": funziona identico su Access e su SQL Server.
    ''' </summary>
    <ApiController>
    <Route("api/clienti")>
    <Produces("application/json")>
    Public Class ClientiController
        Inherits ControllerBase

        Private Const MaxPageSize As Integer = 200

        Private ReadOnly _db As AppDbContext

        Public Sub New(db As AppDbContext)
            _db = db
        End Sub

        ' GET api/clienti?page=1&pageSize=50
        <HttpGet>
        <ProducesResponseType(GetType(PagedResult(Of ClienteDto)), StatusCodes.Status200OK)>
        Public Async Function GetAll(Optional page As Integer = 1,
                                     Optional pageSize As Integer = 50,
                                     Optional cancellationToken As CancellationToken = Nothing) As Task(Of IActionResult)

            page = Math.Max(page, 1)
            pageSize = Math.Clamp(pageSize, 1, MaxPageSize)

            ' OrderBy obbligatorio con la paginazione: senza, l'ordine non e' garantito.
            Dim query = _db.Clienti.AsNoTracking().OrderBy(Function(c) c.Id)

            Dim total = Await query.CountAsync(cancellationToken)
            Dim items = Await query _
                .Skip((page - 1) * pageSize) _
                .Take(pageSize) _
                .Select(ClienteDto.Projection) _
                .ToListAsync(cancellationToken)

            Return Ok(New PagedResult(Of ClienteDto)(items, page, pageSize, total))
        End Function

        ' GET api/clienti/5
        <HttpGet("{id:int}")>
        <ProducesResponseType(GetType(ClienteDto), StatusCodes.Status200OK)>
        <ProducesResponseType(StatusCodes.Status404NotFound)>
        Public Async Function GetById(id As Integer,
                                      Optional cancellationToken As CancellationToken = Nothing) As Task(Of IActionResult)

            Dim dto = Await _db.Clienti.AsNoTracking() _
                .Where(Function(c) c.Id = id) _
                .Select(ClienteDto.Projection) _
                .FirstOrDefaultAsync(cancellationToken)

            If dto Is Nothing Then Return NotFound()
            Return Ok(dto)
        End Function

        ' GET api/clienti/5/ordini
        <HttpGet("{id:int}/ordini")>
        <ProducesResponseType(GetType(List(Of OrdineDto)), StatusCodes.Status200OK)>
        <ProducesResponseType(StatusCodes.Status404NotFound)>
        Public Async Function GetOrdini(id As Integer,
                                        Optional cancellationToken As CancellationToken = Nothing) As Task(Of IActionResult)

            Dim esiste = Await _db.Clienti.AnyAsync(Function(c) c.Id = id, cancellationToken)
            If Not esiste Then Return NotFound()

            Dim ordini = Await _db.Ordini.AsNoTracking() _
                .Where(Function(o) o.ClienteId = id) _
                .OrderByDescending(Function(o) o.DataOrdine) _
                .Select(OrdineDto.Projection) _
                .ToListAsync(cancellationToken)

            Return Ok(ordini)
        End Function

        ' POST api/clienti
        <HttpPost>
        <ProducesResponseType(GetType(ClienteDto), StatusCodes.Status201Created)>
        <ProducesResponseType(StatusCodes.Status400BadRequest)>
        Public Async Function Create(request As ClienteRequest,
                                     Optional cancellationToken As CancellationToken = Nothing) As Task(Of IActionResult)

            Dim entity = New Cliente With {
                .RagioneSociale = request.RagioneSociale,
                .PartitaIva = request.PartitaIva,
                .Email = request.Email,
                .Attivo = request.Attivo,
                .DataInserimento = Date.Now   ' i dati legacy Access sono in ora locale
            }

            _db.Clienti.Add(entity)
            Await _db.SaveChangesAsync(cancellationToken)   ' qui EF legge l'ID generato (AutoNumber/IDENTITY)

            Dim dto = ClienteDto.FromEntity(entity)
            Return CreatedAtAction(NameOf(GetById), New With {.id = entity.Id}, dto)
        End Function

        ' PUT api/clienti/5
        <HttpPut("{id:int}")>
        <ProducesResponseType(StatusCodes.Status204NoContent)>
        <ProducesResponseType(StatusCodes.Status404NotFound)>
        Public Async Function Update(id As Integer,
                                     request As ClienteRequest,
                                     Optional cancellationToken As CancellationToken = Nothing) As Task(Of IActionResult)

            Dim entity = Await _db.Clienti.FirstOrDefaultAsync(Function(c) c.Id = id, cancellationToken)
            If entity Is Nothing Then Return NotFound()

            entity.RagioneSociale = request.RagioneSociale
            entity.PartitaIva = request.PartitaIva
            entity.Email = request.Email
            entity.Attivo = request.Attivo

            Await _db.SaveChangesAsync(cancellationToken)
            Return NoContent()
        End Function

        ' DELETE api/clienti/5
        <HttpDelete("{id:int}")>
        <ProducesResponseType(StatusCodes.Status204NoContent)>
        <ProducesResponseType(StatusCodes.Status404NotFound)>
        <ProducesResponseType(StatusCodes.Status409Conflict)>
        Public Async Function Delete(id As Integer,
                                     Optional cancellationToken As CancellationToken = Nothing) As Task(Of IActionResult)

            Dim entity = Await _db.Clienti.FirstOrDefaultAsync(Function(c) c.Id = id, cancellationToken)
            If entity Is Nothing Then Return NotFound()

            ' Controllo applicativo invece di affidarsi al messaggio d'errore del vincolo,
            ' che e' diverso tra Access e SQL Server.
            Dim haOrdini = Await _db.Ordini.AnyAsync(Function(o) o.ClienteId = id, cancellationToken)
            If haOrdini Then
                Return Problem(statusCode:=StatusCodes.Status409Conflict,
                               title:="Cliente con ordini",
                               detail:="Impossibile eliminare un cliente che ha ordini associati.")
            End If

            _db.Clienti.Remove(entity)
            Await _db.SaveChangesAsync(cancellationToken)
            Return NoContent()
        End Function

    End Class

End Namespace
