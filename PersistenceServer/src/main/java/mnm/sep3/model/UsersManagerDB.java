package mnm.sep3.model;

import mnm.sep3.database.Database;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.HashMap;
import java.util.Map;

public class UsersManagerDB implements UsersManager
{
    private final Connection connection = Database.getConnection();

    private final Map<Integer, User> users = new HashMap<>();

    /**
     * Kræver ResultSet har id: int, username: string og password: string
     * @param resultSet ResultSet fra SQL statement
     */
    private User convertResultSetToUser(ResultSet resultSet) throws SQLException {
        int id = resultSet.getInt("id");
        String username = resultSet.getString("username");
        String password = resultSet.getString("password");

        return new User(id, username, password);
    }

    @Override
    public User createUser(String username, String password) {
        try {
            PreparedStatement statement = connection.prepareStatement("INSERT INTO users (username, password) VALUES (?, ?) RETURNING id");
            statement.setString(1, username);
            statement.setString(2, password);

            ResultSet res = statement.executeQuery();
            if (res.next()) {
                int id = res.getInt("id");
                User returnUser = new User(id, username, password);
                users.put(id, returnUser);
                return returnUser;
            }

            throw new RuntimeException("Kunne ikke tilføje bruger");
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public User getUserByUsername(String username) {
        try {
            PreparedStatement statement = connection.prepareStatement("SELECT * FROM sep3_eksamen.users WHERE username = ? ");
            statement.setString(1, username);

            ResultSet res = statement.executeQuery();
            if (res.next()) {
                User returnUser = convertResultSetToUser(res);
                users.put(returnUser.getId(), returnUser);
                return returnUser;
            }

            throw new RuntimeException("Kunne ikke finde bruger med matchende brugernavn");
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public User getUserById(int id) {
        User returnUser = users.get(id);
        if (returnUser != null) return returnUser;

        try {
            PreparedStatement statement = connection.prepareStatement("SELECT * FROM sep3_eksamen.users WHERE id = ?");
            statement.setInt(1, id);

            ResultSet res = statement.executeQuery();
            if (res.next()) {
                returnUser = convertResultSetToUser(res);
                users.put(returnUser.getId(), returnUser);
                return returnUser;
            }

            throw new RuntimeException("Kunne ikke finde bruger med matchende id");
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void updateUsername(int id, String newUsername) {
        try {
            PreparedStatement statement = connection.prepareStatement("UPDATE sep3_eksamen.users SET username = ? WHERE id = ?");
            statement.setString(1, newUsername);
            statement.setInt(2, id);

            statement.execute();

            User cachedUser = users.get(id);
            if (cachedUser != null) {
                users.put(id, new User(id, newUsername, cachedUser.getPassword()));
            }
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void updatePassword(int id, String password) {
        try {
            PreparedStatement statement = connection.prepareStatement("UPDATE sep3_eksamen.users SET password = ? WHERE id = ?");
            statement.setString(1, password);
            statement.setInt(2, id);

            statement.execute();

            User cachedUser = users.get(id);
            if (cachedUser != null) {
                users.put(id, new User(id, cachedUser.getUsername(), password));
            }
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void deleteUser(int id) {
        try {
            PreparedStatement statement = connection.prepareStatement("DELETE FROM sep3_eksamen.users WHERE id = ?");
            statement.setInt(1, id);

            statement.execute();

            users.remove(id);
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public User verifyUserCredentials(String username, String password) {
        try {
            PreparedStatement statement = connection.prepareStatement("SELECT * FROM sep3_eksamen.users WHERE username = ? AND password = ?");
            statement.setString(1, username);
            statement.setString(2, password);

            ResultSet res = statement.executeQuery();
            if (res.next()) {
                User returnUser = convertResultSetToUser(res);
                users.put(returnUser.getId(), returnUser);
                return returnUser;
            }

            throw new RuntimeException("Forkert brugernavn eller password");
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }
}
