namespace ApiContracts;

public class QuestionDTO
{
    public string Question { get; set; }
    public string Answer { get; set; }
    public int Id { get; set; }
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