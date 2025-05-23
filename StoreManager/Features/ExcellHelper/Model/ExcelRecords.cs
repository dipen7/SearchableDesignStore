namespace StoreManager.Features.ExcellHelper.Model
{
    public class ExcelRecords<T>
    {
        public ExcelRecords()
        {
            Records = new List<T>();
            FailedIndices = new List<int>();
            ReadError = false;
        }
        public List<T> Records { get; set; }
        public List<int> FailedIndices { get; set; }
        public bool ReadError { get; set; }
    }
}
