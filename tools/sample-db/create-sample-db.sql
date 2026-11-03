-- Crea il database LegacyApp su SQL Server con lo stesso schema del DB Access di esempio
-- (tblClienti, tblOrdini: nomi mappati in src/LegacyApp.Data/Configurations) e dati fittizi riproducibili.
-- Equivalente SQL Server di tools/sample-db/create-sample-db.ps1 (che richiede Windows + driver ACE).
--
-- Uso (SQL Server in Docker, vedi BLAZOR.md / README):
--   docker exec -i legacy-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C < tools/sample-db/create-sample-db.sql
-- Idempotente: se il database esiste viene ricreato da zero.

SET NOCOUNT ON;
GO
IF DB_ID('LegacyApp') IS NOT NULL
BEGIN
    ALTER DATABASE LegacyApp SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE LegacyApp;
END
GO
CREATE DATABASE LegacyApp;
GO
USE LegacyApp;
GO

CREATE TABLE dbo.tblClienti (
    IDCliente       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_tblClienti PRIMARY KEY,
    RagioneSociale  NVARCHAR(255) NOT NULL,
    PartitaIVA      NVARCHAR(11)  NULL,
    Email           NVARCHAR(255) NULL,
    DataInserimento DATETIME2     NOT NULL,
    Attivo          BIT           NOT NULL CONSTRAINT DF_tblClienti_Attivo DEFAULT (1)
);
CREATE INDEX IX_tblClienti_PartitaIVA ON dbo.tblClienti (PartitaIVA);

CREATE TABLE dbo.tblOrdini (
    IDOrdine   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_tblOrdini PRIMARY KEY,
    IDCliente  INT           NOT NULL,
    DataOrdine DATETIME2     NOT NULL,
    Importo    DECIMAL(19,4) NOT NULL,
    Note       NVARCHAR(MAX) NULL,
    CONSTRAINT FK_tblOrdini_tblClienti FOREIGN KEY (IDCliente) REFERENCES dbo.tblClienti (IDCliente)
);
CREATE INDEX IX_tblOrdini_IDCliente ON dbo.tblOrdini (IDCliente);
GO

-- 120 clienti con dati deterministici (nessun RAND): ogni esecuzione produce lo stesso DB.
DECLARE @i INT = 1;
WHILE @i <= 120
BEGIN
    INSERT INTO dbo.tblClienti (RagioneSociale, PartitaIVA, Email, DataInserimento, Attivo)
    VALUES (
        CHOOSE((@i % 6) + 1, N'Rossi', N'Bianchi', N'Verdi', N'Esposito', N'Colombo', N'Ferrari')
            + N' ' + CHOOSE((@i % 5) + 1, N'S.r.l.', N'S.p.A.', N'S.n.c.', N'& C. S.a.s.', N'S.r.l.s.')
            + N' ' + RIGHT(N'000' + CAST(@i AS NVARCHAR(3)), 3),
        CASE WHEN @i % 7 = 0 THEN NULL ELSE RIGHT(N'00000000000' + CAST(10000000000 + @i * 7919 AS NVARCHAR(11)), 11) END,
        CASE WHEN @i % 5 = 0 THEN NULL ELSE N'info' + CAST(@i AS NVARCHAR(3)) + N'@esempio.it' END,
        DATEADD(DAY, -(@i * 11 % 900), '2026-01-15T09:30:00'),
        CASE WHEN @i % 9 = 0 THEN 0 ELSE 1 END
    );
    SET @i += 1;
END;

-- Circa 300 ordini; un terzo dei clienti (IDCliente % 3 = 0) resta senza ordini
-- per provare sia l'eliminazione riuscita sia il 409.
DECLARE @c INT = 1;
WHILE @c <= 120
BEGIN
    IF @c % 3 <> 0
    BEGIN
        DECLARE @n INT = (@c % 5) + 1, @k INT = 1;
        WHILE @k <= @n
        BEGIN
            INSERT INTO dbo.tblOrdini (IDCliente, DataOrdine, Importo, Note)
            VALUES (
                @c,
                DATEADD(DAY, -((@c * 13 + @k * 29) % 400), '2026-03-01'),
                CAST(((@c * 37 + @k * 101) % 5000) + 50 AS DECIMAL(19,4)) + 0.4,
                CASE WHEN @k % 2 = 0 THEN N'Ordine n. ' + CAST(@k AS NVARCHAR(2)) + N' del cliente ' + CAST(@c AS NVARCHAR(3)) ELSE NULL END
            );
            SET @k += 1;
        END;
    END;
    SET @c += 1;
END;
GO

SELECT (SELECT COUNT(*) FROM dbo.tblClienti) AS Clienti, (SELECT COUNT(*) FROM dbo.tblOrdini) AS Ordini;
GO
