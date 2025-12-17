using ApiContracts;
using static HostApp.Utils.Utils;

namespace HostApp.Services;

public class HttpAnswerService(HttpClient httpClient) : IAnswerService
{
    public async Task<List<AnswerDTO>> GetAnswersFromQuestionAsync(int questionId)
    {
        var res = await httpClient.GetAsync($"answers?questionId={questionId}");
        await CheckResponse(res);
        
        var content = await res.Content.ReadFromJsonAsync<List<AnswerDTO>>();
        if (content == null) throw new NullReferenceException($"Response is null");
        return content;
    }

    public async Task<AnswerDTO> AddAnswerAsync(CreateAnswerDTO answer)
    {
        var res = await httpClient.PostAsJsonAsync($"answers", answer);
        await CheckResponse(res);
        
        var content = await res.Content.ReadFromJsonAsync<AnswerDTO>();
        if (content == null) throw new NullReferenceException($"Response is null");
        return content;
    }

    public async Task UpdateAnswerAsync(AnswerDTO answer)
    {
        var res = await httpClient.PostAsJsonAsync($"answers/{answer.AnswerId}", answer);
        await CheckResponse(res);
    }

    public async Task DeleteAnswerAsync(int answerId)
    {
        var res = await httpClient.DeleteAsync($"answers/{answerId}");
        await CheckResponse(res);
    }
}