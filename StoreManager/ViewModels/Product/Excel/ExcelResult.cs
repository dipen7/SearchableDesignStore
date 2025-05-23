namespace StoreManager.ViewModels.Product.Excel
{
    public class ExcelResult
    {
        public ExcelResult()
        {
            FailedIndices = new List<int>();
            ReadError = false;
            DbError = false;
        }
        public List<int> FailedIndices { get; set; }
        public bool ReadError { get; set; }
        public bool DbError { get; set; }
        public string Message { get; set; }
    }
}
