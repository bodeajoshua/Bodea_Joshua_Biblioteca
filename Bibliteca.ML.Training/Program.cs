
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

var ml = new MLContext(seed: 1);

IDataView data = ml.Data.LoadFromTextFile<BookRating>(
    "book-ratings.csv",
    hasHeader: true,
    separatorChar: ',');

var split = ml.Data.TrainTestSplit(
    data, testFraction: 0.2);

var options = new MatrixFactorizationTrainer.Options
{
    MatrixColumnIndexColumnName = "UserKey",
    MatrixRowIndexColumnName = "BookKey",
    LabelColumnName = "Label",
    NumberOfIterations = 50,
    ApproximationRank = 16
};

var pipeline = ml.Transforms.Conversion
    .MapValueToKey("UserKey", "UserId")
    .Append(ml.Transforms.Conversion
        .MapValueToKey("BookKey", "BookId"))
    .Append(ml.Recommendation()
        .Trainers.MatrixFactorization(options));

ITransformer model = pipeline.Fit(split.TrainSet);

var m = ml.Regression.Evaluate(
    model.Transform(split.TestSet));

Console.WriteLine($"RMSE = {m.RootMeanSquaredError:0.00}");
Console.WriteLine($"R² = {m.RSquared:0.00}");

ml.Model.Save(
    model,
    data.Schema,
    "BookRecommender.zip");

public class BookRating
{
    [LoadColumn(0)]
    public float UserId { get; set; }

    [LoadColumn(1)]
    public float BookId { get; set; }

    [LoadColumn(2), ColumnName("Label")]
    public float Rating { get; set; }
}

public class BookRatingPrediction
{
    public float Score { get; set; }
}
