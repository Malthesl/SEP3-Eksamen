namespace ApiContracts;

public class QuestionDTO
{
    public int Id { get; init; }
    public string Question { get; set; }
    public string Answer { get; set; }
}

public class QuestionListDTO
{
    public List<QuestionDTO> Questions { get; set; }
}

public class CreateQuestionDTO
{
    public string Question { get; set; }
    public string Answer { get; set; }
}