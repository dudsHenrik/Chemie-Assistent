CREATE VIEW vw_CamadaValencia AS
WITH NivelMaximo AS (
    SELECT ce.Simbolo, MAX(se.NivelPrincipal) AS NivelPrincipalMax
    FROM ConfiguracaoEletronica ce
    JOIN SubnivelEletronico se ON se.Codigo = ce.Subnivel
    GROUP BY ce.Simbolo
)
SELECT
    ce.Simbolo,
    nm.NivelPrincipalMax AS Nivel,
    SUM(ce.Eletrons) AS CamadaValencia
FROM ConfiguracaoEletronica ce
JOIN SubnivelEletronico se ON se.Codigo = ce.Subnivel
JOIN NivelMaximo nm ON nm.Simbolo = ce.Simbolo AND se.NivelPrincipal = nm.NivelPrincipalMax
GROUP BY ce.Simbolo, nm.NivelPrincipalMax;
GO

SELECT * FROM vw_CamadaValencia ORDER BY Nivel, Simbolo;
GO