using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace AccessHub.API.Services;

public class ParameterStoreService
{
    private readonly IAmazonSimpleSystemsManagement _ssm;

    public ParameterStoreService(IAmazonSimpleSystemsManagement ssm)
    {
        _ssm = ssm;
    }

    public async Task<string> GetParameterAsync(string parameterName)
    {
        var response = await _ssm.GetParameterAsync(
            new GetParameterRequest { Name = parameterName, WithDecryption = true }
        );

        return response.Parameter.Value;
    }
}
