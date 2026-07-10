namespace ProductExporter.DataWriter;

public sealed class CsvFileDataWriter
{
    private readonly string _filePath;
    private readonly Services.Interfaces.IDataTableReader _dataTableReader;

    public CsvFileDataWriter(FileInfo fileInfo, Services.Interfaces.IDataTableReader dataTableReader)
    {
        _filePath = fileInfo.FullName ?? throw new ArgumentNullException(nameof(fileInfo.FullName));
        _dataTableReader = dataTableReader ?? throw new ArgumentNullException(nameof(dataTableReader));
    }

    public void WriteDataToFile()
    {
        using var fileStream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Write);
        using var writer = new StreamWriter(fileStream);

        for (int i = 0; i < _dataTableReader.GetData().Rows.Count; i++)
        {
            var row = _dataTableReader.GetData().Rows[i];

            var categoryName = row["CategoryName"].ToString();
            var categoryIsActive = Convert.ToBoolean(row["CategoryIsActive"]);
            var productCode = Convert.ToInt32(row["ProductCode"]);
            var productName = row["ProductName"].ToString();
            var productQuantity = Convert.ToInt32(row["ProductQuantity"]);
            var productPrice = Convert.ToDecimal(row["ProductPrice"]);
            var productIsActive = Convert.ToBoolean(row["ProductIsActive"]);

            writer.WriteLine(
                             $"{categoryName}\t" +
                             $"{(categoryIsActive ? 1 : 0)}\t" +
                             $"{productCode}\t" +
                             $"{productName}\t" +
                             $"{productQuantity}\t" +
                             $"{productPrice}\t" +
                             $"{(productIsActive ? 1 : 0)}"
                             );
        }
    }
}
