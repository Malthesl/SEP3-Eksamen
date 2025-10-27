package mnm.sep3;

import io.grpc.Server;
import io.grpc.ServerBuilder;
import mnm.sep3.database.Database;
import mnm.sep3.model.QuestionsManager;
import mnm.sep3.model.QuestionsManagerDB;
import mnm.sep3.server.ProofOfConceptImpl;

public class Main {
  public static void main(String[] args)
  {
    QuestionsManager questionsManager = new QuestionsManagerDB();

    Server server = ServerBuilder.forPort(7042).addService(new ProofOfConceptImpl(questionsManager)).build();
    try {
      System.out.println("grpc server starter");
      server.start();
      server.awaitTermination();
    } catch (Exception e) {
      throw new RuntimeException(e);
    }
  }
}