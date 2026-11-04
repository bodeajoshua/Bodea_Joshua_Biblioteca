
using Microsoft.ML.Data;

namespace sem1.Models;

public class BookRating
{
    [LoadColumn(0)]
    public float UserId { get; set; }

    [LoadColumn(1)]
    public float BookId { get; set; }

    [LoadColumn(2), ColumnName("Label")]
    public float Rating { get; set; }
}
