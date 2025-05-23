using OfficeOpenXml;
using StoreManager.Features.ExcellHelper.Model;
using StoreManager.ViewModels.Product;
using System.Net.Mail;

namespace StoreManager.Features.ExcellHelper
{
    public class ExcellHelper : IExcellHelper
    {
        //private readonly Logger<IExcellHelper> _logger;
        public ExcellHelper(/*Logger<IExcellHelper> logger*/)
        {
            //_logger = logger;
        }
        public ExcelRecords<ProductVM> ReadProductsFromCSV(IFormFile file)
        {
            ExcelRecords<ProductVM> excelRecords = new ExcelRecords<ProductVM>();
            try
            {
                List<ProductVM> productVMs = new List<ProductVM>();
                List<ProductVM> records = new List<ProductVM>();
                using var reader = new StreamReader(file.OpenReadStream());
                int row = 0;

                while (!reader.EndOfStream)
                {
                    try
                    {
                        var line = reader.ReadLine();
                        row++;

                        if (row == 1) 
                            continue; // Skip header

                        var values = line.Split(',');
                        int expectedColumnCount = 10;
                        if (values.Length < expectedColumnCount)
                        {
                            throw new Exception("empty cell founded.");
                        }

                        for (int col = 0; col < values.Length; col++)
                        {
                            if (col != 1 && col != 9)
                                if (string.IsNullOrWhiteSpace(values[col]))
                                {
                                    throw new Exception("empty cell founded.");
                                }
                        }
                        if (!IsValidEmail(values[2]))
                        {
                            throw new Exception("Invalid supplier Email.");

                        }

                        ProductVM excelRecord = new ProductVM();
                        excelRecord.ProductName = values[0];
                        excelRecord.Description = values[1];
                        excelRecord.SupplierEmail = values[2];
                        excelRecord.Category = values[3];
                        excelRecord.ManufacturedDate = DateTime.Parse(values[4]);
                        excelRecord.StockQuantity = int.Parse(values[5]);
                        excelRecord.IsActive = bool.Parse(values[6]);
                        excelRecord.CreatedDate = DateTime.Parse(values[7]);
                        excelRecord.Price = int.Parse(values[8]);
                        excelRecord.ExpiryDate = string.IsNullOrWhiteSpace(values[9]) ? null : DateTime.Parse(values[9]);

                        productVMs.Add(excelRecord);

                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex.Message);
                        excelRecords.FailedIndices.Add(row);
                    }

                }
                excelRecords.Records = productVMs;

            }
            catch(Exception ex) 
            {
                //_logger.LogError(ex.Message);
                excelRecords.ReadError = true;

            }
            return excelRecords;

        }

        public ExcelRecords<ProductVM> ReadProductsFromExcel(IFormFile file)
        {
            ExcelRecords<ProductVM> excelRecords = new ExcelRecords<ProductVM>();
            try
            {
                List<ProductVM> productVMs = new List<ProductVM>();
                ExcelPackage.License.SetNonCommercialPersonal("Dipen");
                //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using var stream = new MemoryStream();
                file.CopyTo(stream);
                using var package = new ExcelPackage(stream);

                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    throw new Exception("The Excel file is empty.");
                }

                int rowCount = worksheet.Dimension?.Rows ?? 0;
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        for (int col = 1; col <= 9; col++)
                        {
                            if(col!=2 && col!=10)
                            if (string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Text))
                                throw new Exception("empty cell founded.");
                        }
                        if(!IsValidEmail(worksheet.Cells[row, 3].Text))
                            throw new Exception("Invalid supplier Email.");

                        ProductVM excelRecord = new ProductVM();
                        excelRecord.ProductName = worksheet.Cells[row, 1].Text;
                        excelRecord.Description = worksheet.Cells[row, 2].Text;
                        excelRecord.SupplierEmail = worksheet.Cells[row, 3].Text;
                        excelRecord.Category = worksheet.Cells[row, 4].Text;
                        excelRecord.ManufacturedDate = DateTime.Parse(worksheet.Cells[row, 5].Text);
                        excelRecord.StockQuantity = int.Parse(worksheet.Cells[row, 6].Text);
                        excelRecord.IsActive = bool.Parse(worksheet.Cells[row, 7].Text);
                        excelRecord.CreatedDate = DateTime.Parse(worksheet.Cells[row, 8].Text);
                        excelRecord.Price = int.Parse(worksheet.Cells[row, 9].Text);
                        excelRecord.ExpiryDate = string.IsNullOrEmpty(worksheet.Cells[row, 10].Text) ?null: DateTime.Parse(worksheet.Cells[row, 10].Text);

                        productVMs.Add(excelRecord);
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex.Message);
                        excelRecords.FailedIndices.Add(row);
                    }
                }
                excelRecords.Records = productVMs;

            }
            catch(Exception ex)
            {
                //_logger.LogError(ex.Message);
                excelRecords.ReadError = true;

            }
            return excelRecords;

        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
