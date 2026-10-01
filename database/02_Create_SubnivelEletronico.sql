CREATE TABLE SubnivelEletronico (
    Codigo           VARCHAR(3) NOT NULL,
    NivelPrincipal   INT        NOT NULL,
    TipoOrbital      CHAR(1)    NOT NULL,
    CapacidadeMaxima INT        NOT NULL,
    Ordem            INT        NOT NULL,

    CONSTRAINT PK_SubnivelEletronico PRIMARY KEY NONCLUSTERED (Codigo),
    CONSTRAINT UK_SubnivelEletronico UNIQUE (Ordem)
);
GO

CREATE CLUSTERED INDEX IX_SubnivelEletronico_Ordem
ON SubnivelEletronico (Ordem);
GO

INSERT INTO SubnivelEletronico (Codigo, NivelPrincipal, TipoOrbital, CapacidadeMaxima, Ordem)
VALUES
    ('1s', 1, 's',  2,  1),
    ('2s', 2, 's',  2,  2),
    ('2p', 2, 'p',  6,  3),
    ('3s', 3, 's',  2,  4),
    ('3p', 3, 'p',  6,  5),
    ('3d', 3, 'd', 10,  7),
    ('4s', 4, 's',  2,  6),
    ('4p', 4, 'p',  6,  8),
    ('4d', 4, 'd', 10, 10),
    ('4f', 4, 'f', 14, 13),
    ('5s', 5, 's',  2,  9),
    ('5p', 5, 'p',  6, 11),
    ('5d', 5, 'd', 10, 14),
    ('5f', 5, 'f', 14, 17),
    ('6s', 6, 's',  2, 12),
    ('6p', 6, 'p',  6, 15),
    ('6d', 6, 'd', 10, 18),
    ('7s', 7, 's',  2, 16),
    ('7p', 7, 'p',  6, 19);
GO

SELECT * FROM SubnivelEletronico;
GO