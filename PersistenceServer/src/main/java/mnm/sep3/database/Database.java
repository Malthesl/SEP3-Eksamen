package mnm.sep3.database;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

public class Database
{
  private static Connection context;

  public static Connection getConnection() {
    try {
      if (context == null) {
        context = DriverManager.getConnection("jdbc:postgresql://localhost:5432/sep3_eksamen", "sep3_eksamen", "sep3_eksamen");
        context.setSchema("sep3_eksamen");
        return context;
      }
      return context;
    }
    catch (SQLException exception) {
      exception.printStackTrace();
      throw new RuntimeException(exception.getMessage());
    }
  }
}
