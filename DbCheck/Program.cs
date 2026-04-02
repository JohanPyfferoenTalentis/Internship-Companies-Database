using Microsoft.Data.Sqlite;
using ClosedXML.Excel;
using Microsoft.Data.Sqlite;

var xlsxPath = Path.Combine("InternshipDB", "companies", "tblCompanies.xlsx");
Console.WriteLine("Excel exists: " + File.Exists(xlsxPath));

if (File.Exists(xlsxPath))
{
    using var wb = new XLWorkbook(xlsxPath);
    var ws = wb.Worksheets.First();
    var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;
    Console.WriteLine($"Total columns: {lastCol}");

    // Print headers
    var hdr = ws.Row(1);
    for (int c = 1; c <= lastCol; c++)
        Console.Write($"[{c}]={hdr.Cell(c).GetString()}  ");
    Console.WriteLine();

    // Print first 8 data rows with all columns
    Console.WriteLine("\n--- First 8 Excel rows ---");
    foreach (var row in ws.RowsUsed().Skip(1).Take(8))
    {
        Console.WriteLine($"Row {row.RowNumber()}:");
        for (int c = 1; c <= lastCol; c++)
        {
            var v = row.Cell(c).GetString()?.Trim();
            if (!string.IsNullOrWhiteSpace(v))
                Console.WriteLine($"  [{c}] {hdr.Cell(c).GetString()} = {v}");
        }
        Console.WriteLine();
    }

    var total = ws.RowsUsed().Skip(1).Count();
    var withName = ws.RowsUsed().Skip(1).Count(r => !string.IsNullOrWhiteSpace(r.Cell(2).GetString()));
    Console.WriteLine($"Total Excel rows: {total}, With company name: {withName}");
}
else
{
    Console.WriteLine("Excel file not found!");
}

// Database comparison
Console.WriteLine("\n--- Database ---");
var conn = new SqliteConnection("Data Source=InternshipDB/companies.db");
conn.Open();
var cmd = conn.CreateCommand();
cmd.CommandText = "SELECT COUNT(*) FROM Companies";
Console.WriteLine("DB total: " + cmd.ExecuteScalar());
cmd.CommandText = "SELECT COUNT(*) FROM Companies WHERE CompanyName = 'Unknown'";
Console.WriteLine("DB Unknown: " + cmd.ExecuteScalar());

Console.WriteLine("\n--- First 5 DB rows ---");
cmd.CommandText = @"SELECT Id, CompanyName, CompanyRegistrationNumber, Sector, PersonInCharge, Address, ContactNumber, Email, Website, InternshipPeriod, DressCode, Information 
FROM Companies WHERE CompanyName != 'Unknown' AND CompanyName IS NOT NULL ORDER BY Id LIMIT 5";
var r = cmd.ExecuteReader();
while (r.Read())
{
    Console.WriteLine($"ID={r[0]} | Name={r[1]}");
    Console.WriteLine($"  RegNo={r[2]} | Sector={r[3]} | Person={r[4]}");
    Console.WriteLine($"  Addr={r[5]} | Phone={r[6]} | Email={r[7]}");
    Console.WriteLine($"  Web={r[8]} | Period={r[9]} | Dress={r[10]} | Info={r[11]}");
    Console.WriteLine();
}
r.Close();
conn.Close();
