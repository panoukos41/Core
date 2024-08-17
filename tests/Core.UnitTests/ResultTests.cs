using System.Text.Json;

namespace Core;

public sealed class ResultTests
{
    [Fact]
    public void ShouldConvertToJson()
    {
        Result<int[]> result;
        Result<Nothing> result2;

        result = new[] { 10, 20, 30 };
        result2 = Nothing.Value;

        var json = JsonSerializer.Serialize(result, result.GetType(), Options.Json);
        var json2 = JsonSerializer.Serialize(result2, result2.GetType(), Options.Json);

        var r = JsonSerializer.Deserialize<Result<int[]>>(json, Options.Json);
        var r2 = JsonSerializer.Deserialize<Result<Nothing>>(json2, Options.Json);

        Assert.True(true);

    }

    [Fact]
    public void ShouldChain()
    {
        var initial = Result.Run(GetNumber);

        var final = initial
            .Chain((num) =>
            {
                Result<string> r = num is 0 ? Problems.Validation : "noice";
                return r;
            })
            .Chain<string, bool>(num =>
            {
                return num is not "0" ? true : Problems.Validation;
            })
            .Chain<bool, string, ResultTests>(this, static (@this, num) =>
            {
                return num.ToString();
            });

        var v = final.Match(ok => ok.Value.ToString(), er => er.Problem.ToString());


    }

    private int GetNumber()
    {
        //throw new NotImplementedException();
        //return 0;
        return 11;
    }
}
