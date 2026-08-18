CREATE TABLE Cliente
(
    CodCliente INT IDENTITY(1,1) NOT NULL,
    CNPJ VARCHAR(14) NOT NULL,
    Nome VARCHAR(150) NOT NULL,
    Email VARCHAR(150) NOT NULL,
    DataCadastro DATETIME2 NOT NULL
        CONSTRAINT DF_Cliente_DataCadastro
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Cliente
        PRIMARY KEY (CodCliente),

    CONSTRAINT UQ_Cliente_CNPJ
        UNIQUE (CNPJ)
);

CREATE TABLE Produto
(
    CodProduto INT IDENTITY(1,1) NOT NULL,
    Nome VARCHAR(150) NOT NULL,
    Preco DECIMAL(18,2) NOT NULL,
    Estoque INT NOT NULL,

    CONSTRAINT PK_Produto
        PRIMARY KEY (CodProduto),

    CONSTRAINT CK_Produto_Preco
        CHECK (Preco >= 0),

    CONSTRAINT CK_Produto_Estoque
        CHECK (Estoque >= 0)
);

CREATE TABLE Pedido
(
    CodPedido INT IDENTITY(1,1) NOT NULL,
    CodCliente INT NOT NULL,
    DataPedido DATETIME2 NOT NULL
        CONSTRAINT DF_Pedido_DataPedido
        DEFAULT SYSUTCDATETIME(),
    ValorTotal DECIMAL(18,2) NOT NULL
        CONSTRAINT DF_Pedido_ValorTotal
        DEFAULT 0,

    CONSTRAINT PK_Pedido
        PRIMARY KEY (CodPedido),

    CONSTRAINT FK_Pedido_Cliente
        FOREIGN KEY (CodCliente)
        REFERENCES Cliente (CodCliente),

    CONSTRAINT CK_Pedido_ValorTotal
        CHECK (ValorTotal >= 0)
);

CREATE TABLE ItensPedido
(
    CodPedido INT NOT NULL,
    CodProduto INT NOT NULL,
    Quantidade INT NOT NULL,
    PrecoUnitario DECIMAL(18,2) NOT NULL,

    CONSTRAINT PK_ItensPedido
        PRIMARY KEY (CodPedido, CodProduto),

    CONSTRAINT FK_ItensPedido_Pedido
        FOREIGN KEY (CodPedido)
        REFERENCES Pedido (CodPedido),

    CONSTRAINT FK_ItensPedido_Produto
        FOREIGN KEY (CodProduto)
        REFERENCES Produto (CodProduto),

    CONSTRAINT CK_ItensPedido_Quantidade
        CHECK (Quantidade > 0),

    CONSTRAINT CK_ItensPedido_PrecoUnitario
        CHECK (PrecoUnitario >= 0)
);

CREATE INDEX IX_Cliente_CNPJ
    ON Cliente (CNPJ);

CREATE INDEX IX_Pedido_CodCliente
    ON Pedido (CodCliente);

CREATE INDEX IX_Pedido_DataPedido
    ON Pedido (DataPedido);

CREATE INDEX IX_ItensPedido_CodProduto
    ON ItensPedido (CodProduto);