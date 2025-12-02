package mnm.sep3;

import io.grpc.Server;
import io.grpc.ServerBuilder;
import mnm.sep3.model.*;
import mnm.sep3.server.AnswersServiceImpl;
import mnm.sep3.model.*;
import mnm.sep3.server.QuestionsServiceImpl;
import mnm.sep3.server.QuizServiceImpl;
import mnm.sep3.server.UserServiceImpl;

public class Main {
  public static void main(String[] args)
  {
    QuestionsManager questionsManager = new QuestionsManagerDB();
    UsersManager usersManager = new UsersManagerDB();
    QuizzesManager quizzesManager = new QuizzesManagerDB();
    AnswersManager answersManager = new AnswersManagerDB();

    Server server = ServerBuilder.forPort(7042)
            .addService(new QuestionsServiceImpl(questionsManager))
            .addService(new QuizServiceImpl(quizzesManager))
            .addService(new AnswersServiceImpl(answersManager))
            .addService(new UserServiceImpl(usersManager)).build();
    try {
      System.out.println("grpc server starter");
      server.start();
      server.awaitTermination();
    } catch (Exception e) {
      throw new RuntimeException(e);
    }
  }
}