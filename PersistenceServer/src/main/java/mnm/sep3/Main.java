package mnm.sep3;

import io.grpc.Server;
import io.grpc.ServerBuilder;
import mnm.sep3.model.QuestionsManager;
import mnm.sep3.model.QuestionsManagerDB;
import mnm.sep3.model.UsersManager;
import mnm.sep3.model.UsersManagerDB;
import mnm.sep3.server.QuizServiceImpl;
import mnm.sep3.server.UserServiceImpl;

public class Main {
  public static void main(String[] args)
  {
    QuestionsManager questionsManager = new QuestionsManagerDB();
    UsersManager usersManager = new UsersManagerDB();

    Server server = ServerBuilder.forPort(7042)
            .addService(new QuizServiceImpl(questionsManager))
            .addService(new UserServiceImpl(usersManager)).build();
    try {
      System.out.println("grpc server starter");

      questionsManager.moveQuestion(5, 2);
      server.start();
      server.awaitTermination();
    } catch (Exception e) {
      throw new RuntimeException(e);
    }
  }
}