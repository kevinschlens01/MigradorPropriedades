using System.Text.RegularExpressions;


class Program
{
    static void Main()
    {
        string ddl = @"
        CREATE TABLE eldados_transparencia_geral.dbo.vw_portal_gmp_compra_ordem (
            id int IDENTITY(1,1) NOT NULL,
            portal_id int NOT NULL,
            hash_registro nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
            transparencia_hash_cliente nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
            tipo_processo nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            nome_tipo_processo nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            nome_tipo_af nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            codigo_empresa nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            nome_empresa nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            cnpj_empresa nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            codigo_filial nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            nome_filial nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            cnpj_filial nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            descricao_licitacao nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            descricao_contrato nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            codigo_ordem_compra nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            data_ordem_compra date NULL,
            nome_modalidade nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            numero_ordem_compra nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            ano_ordem_compra nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            nome_situacao nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            codigo_secretaria nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            nome_secretaria nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            codigo_local_requerente nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            nome_local_requerente nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            codigo_g_fornecedor nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            nome_g_fornecedor nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            nome_cpf_cnpj nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            cpf_cnpj nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            numero_processo nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            ano_processo nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            descricao_ordco nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            codigo_licitacao nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            codigo_contrato nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            nome_motivo nvarchar(400) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            vlr_total numeric(25,9) NULL,
            ano nvarchar(4) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            mes nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            anexo nvarchar(1) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'N' NULL,
            esfera_administrativa int NULL,
            esfera_poder varchar(1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            esfera_poder_tipo varchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            esfera_poder_ug varchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            codigo_tce varchar(15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            numero_artigo varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            codigo_local_entrega varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            nome_local_entrega nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            hash_registro_area nvarchar(38) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            controle_area nvarchar(60) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
            CONSTRAINT pk_vw_portal_gmp_compra_ordem PRIMARY KEY (id)
        );";

        // Regex pattern to extract column definitions without COLLATE
        string pattern = @"(?<column>\w+)\s+(?<type>\w+(\(\d+(\,\d+)?\))?)\s*(?:COLLATE\s+\S+\s+)?(?<nullable>NULL|NOT NULL)";

        MatchCollection matches = Regex.Matches(ddl, pattern);
        List<string> properties = new List<string>();

        foreach (Match match in matches)
        {
            string columnName = match.Groups["column"].Value;
            string dataType = match.Groups["type"].Value;
            bool isNullable = match.Groups["nullable"].Value == "NULL";

            string csharpType = ConvertToCSharpType(dataType, isNullable);
            string propertyName = ConvertToPascalCase(columnName);

            properties.Add($"public {csharpType} {propertyName} {{ get; set; }}");
        }

        // Output the properties
        Console.WriteLine("Propriedades Geradas:");
        foreach (string property in properties)
        {
            Console.WriteLine(property);
        }
    }

    static string ConvertToCSharpType(string sqlType, bool isNullable)
    {
        string csharpType = sqlType switch
        {
            "int" => "int",
            "nvarchar" => "string",
            "varchar" => "string",
            "date" => "DateTime",
            "numeric" => "decimal",
            _ => "string" // Default to string if no match
        };

        if (csharpType != "string" && isNullable)
        {
            csharpType += "?";
        }

        return csharpType;
    }

    static string ConvertToPascalCase(string input)
    {
        string[] words = input.Split('_');
        for (int i = 0; i < words.Length; i++)
        {
            words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
        }
        return string.Join("", words);
    }
}