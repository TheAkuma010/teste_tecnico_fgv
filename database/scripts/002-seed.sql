USE SalesDb;
GO

IF NOT EXISTS (
    SELECT 1
    FROM Cliente
    WHERE CNPJ = '12345678000199'
)
BEGIN
    INSERT INTO Cliente
    (
        CNPJ,
        Nome,
        Email
    )
    VALUES
    (
        '12345678000199',
        'João da Silva',
        'joao.silva@email.com'
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM Produto
    WHERE Nome = 'Monitor'
)
BEGIN
    INSERT INTO Produto
    (
        Nome,
        Preco,
        Estoque
    )
    VALUES
    (
        'Monitor',
        800.00,
        5
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM Produto
    WHERE Nome = 'Teclado USB'
)
BEGIN
    INSERT INTO Produto
    (
        Nome,
        Preco,
        Estoque
    )
    VALUES
    (
        'Teclado USB',
        150.00,
        0
    );
END;
GO