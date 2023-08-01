using PayPingTechnicalTask.Model.Enums;

namespace PayPingTechnicalTask.Model;

public class BaseResponseModel
{
    public BaseResponseModel()
    {
        Messages = new List<string>();
    }

    public ResultType Result { get; set; }

    public List<string> Messages { get; set; }
}