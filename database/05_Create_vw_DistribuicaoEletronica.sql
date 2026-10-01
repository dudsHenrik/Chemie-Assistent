CREATE VIEW vw_DistribuicaoEletronica AS
SELECT
    ce.Simbolo,
    STRING_AGG(CONCAT(ce.Subnivel, TRANSLATE(CONVERT(NVARCHAR(10), ce.Eletrons), N'0123456789', N'⁰¹²³⁴⁵⁶⁷⁸⁹')), ' ')
        WITHIN GROUP (ORDER BY se.Ordem) AS DistribuicaoLinusPauling,
    cv.CamadaValencia
FROM ConfiguracaoEletronica ce
JOIN SubnivelEletronico se ON se.Codigo = ce.Subnivel
JOIN vw_CamadaValencia cv ON cv.Simbolo = ce.Simbolo
GROUP BY ce.Simbolo, cv.CamadaValencia;
GO

SELECT de.* FROM vw_DistribuicaoEletronica de JOIN Atomo a ON de.Simbolo = a.Simbolo ORDER BY a.NumeroAtomico;
GO